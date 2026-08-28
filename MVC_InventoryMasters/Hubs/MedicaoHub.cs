using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;

namespace MVC_InventoryMasters.Hubs
{
    /// <summary>
    /// Hub responsável por receber as medições enviadas
    /// pelo Kinect, armazená-las no Firestore e distribuir
    /// atualizações em tempo real para os clientes conectados.
    /// </summary>
    public class MedicaoHub : Hub
    {
        private readonly IMedicaoVolumeRepository _medicaoRepository;
        private readonly IParametrosSistemaRepository _parametrosRepository;
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly ILogger<MedicaoHub> _logger;

        /// <summary>
        /// Inicializa uma nova instância do Hub de medições.
        /// </summary>
        /// <param name="medicaoRepository">
        /// Repositório responsável pelo armazenamento das medições.
        /// </param>
        /// <param name="parametrosRepository">
        /// Repositório responsável pelos parâmetros do sistema.
        /// </param>
        /// <param name="notificacaoRepository">
        /// Repositório responsável pelas notificações automáticas.
        /// </param>
        /// <param name="logger">
        /// Serviço de log utilizado para registrar eventos e erros.
        /// </param>
        public MedicaoHub(
            IMedicaoVolumeRepository medicaoRepository,
            IParametrosSistemaRepository parametrosRepository,
            INotificacaoRepository notificacaoRepository,
            ILogger<MedicaoHub> logger)
        {
            _medicaoRepository = medicaoRepository;
            _parametrosRepository = parametrosRepository;
            _notificacaoRepository = notificacaoRepository;
            _logger = logger;
        }

        /// <summary>
        /// Recebe o volume calculado pelo Kinect,
        /// salva a medição e atualiza os clientes
        /// conectados em tempo real.
        /// </summary>
        /// <param name="volumeCm3">
        /// Volume recebido do Kinect em centímetros cúbicos (cm³).
        /// </param>
        /// <returns>
        /// Tarefa assíncrona de processamento da medição.
        /// </returns>
        public async Task EnviarVolume(
            double volumeCm3,
            string? empresaId = null,
            double volumeMaximoCm3 = 0,
            double percentualAlertaKinect = 0)
        {
            if (double.IsNaN(volumeCm3) || double.IsInfinity(volumeCm3) || volumeCm3 < 0)
            {
                await Clients.Caller.SendAsync(
                    "ErroProcessamento",
                    "A medição enviada possui um volume inválido.");
                return;
            }

            double volumeM3 = volumeCm3 / 1000000d;
            DateTime dataHora = DateTime.UtcNow;

            // A atualização visual não deve depender da disponibilidade do Firestore.
            await Clients.All.SendAsync(
                "NovaMedicao",
                new
                {
                    volumeMedido = volumeM3,
                    dataHora = dataHora.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss")
                });

            try
            {
                var medicao = new MedicaoVolume
                {
                    OrigemLeitura = "Kinect",
                    Status = "Normal",
                    VolumeMedido = volumeM3,
                    DataHora = dataHora,
                    EmpresaId = string.IsNullOrWhiteSpace(empresaId) ? null : empresaId
                };

                await _medicaoRepository.Adicionar(medicao);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao processar uma medição recebida pelo Kinect."
                );

                await Clients.Caller.SendAsync(
                    "ErroProcessamento",
                    "A medição foi exibida, mas não pôde ser salva no banco de dados."
                );

                return;
            }

            bool alertaCriado = await VerificarAlertas(
                volumeM3,
                empresaId,
                volumeMaximoCm3,
                percentualAlertaKinect);

            if (alertaCriado)
            {
                await Clients.All.SendAsync("NovaNotificacao");
            }
        }

        /// <summary>
        /// Verifica se o volume atual atingiu os limites
        /// configurados e gera notificações automáticas.
        /// </summary>
        /// <param name="volumeAtual">
        /// Volume atual medido em metros cúbicos (m³).
        /// </param>
        /// <returns>
        /// Tarefa assíncrona de verificação dos alertas.
        /// </returns>
        private async Task<bool> VerificarAlertas(
            double volumeAtual,
            string? empresaId,
            double volumeMaximoCm3,
            double percentualAlertaKinect)
        {
            try
            {
                var parametros = string.IsNullOrWhiteSpace(empresaId)
                    ? _parametrosRepository.Buscar()
                    : _parametrosRepository.BuscarPorEmpresa(empresaId);

                double capacidadeMaxima = volumeMaximoCm3 > 0
                    ? volumeMaximoCm3 / 1000000d
                    : parametros?.CapacidadeMaxima ?? 0;

                double percentualAlerta = percentualAlertaKinect > 0 &&
                                          percentualAlertaKinect <= 100
                    ? percentualAlertaKinect
                    : parametros?.PercentualAlerta ?? 0;

                if (capacidadeMaxima <= 0 || percentualAlerta <= 0)
                {
                    _logger.LogWarning(
                        "Capacidade máxima ou percentual inválido para geração de alertas."
                    );

                    return false;
                }

                double percentual = Math.Min(
                    (volumeAtual / capacidadeMaxima) * 100,
                    100);

                if (percentual < percentualAlerta)
                {
                    return false;
                }

                bool existePendente =
                    await _notificacaoRepository
                        .ExisteNotificacaoPendente(empresaId);

                if (existePendente)
                {
                    return false;
                }

                var notificacao = new Notificacao
                {
                    VolumeMedido = volumeAtual,
                    EmpresaId = string.IsNullOrWhiteSpace(empresaId) ? null : empresaId,
                    Tipo = "Capacidade",
                    Automatica = true,
                    StatusEnvio = "Pendente",
                    Mensagem =
                        $"O estoque atingiu {percentual:F1}% da capacidade máxima."
                };

                await _notificacaoRepository
                    .Adicionar(notificacao);

                _logger.LogInformation(
                    "Notificação automática criada para {Percentual:F1}% de ocupação.",
                    percentual
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao verificar alertas automáticos."
                );

                return false;
            }
        }

        /// <summary>
        /// Executado quando um cliente estabelece conexão
        /// com o Hub de medições.
        /// </summary>
        /// <returns>
        /// Tarefa assíncrona de conexão.
        /// </returns>
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation(
                "Cliente conectado ao Hub. ConnectionId: {ConnectionId}",
                Context.ConnectionId
            );

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Executado quando um cliente encerra conexão
        /// com o Hub de medições.
        /// </summary>
        /// <param name="exception">
        /// Exceção ocorrida durante a desconexão, se houver.
        /// </param>
        /// <returns>
        /// Tarefa assíncrona de desconexão.
        /// </returns>
        public override async Task OnDisconnectedAsync(
            Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogWarning(
                    exception,
                    "Cliente desconectado com erro. ConnectionId: {ConnectionId}",
                    Context.ConnectionId
                );
            }
            else
            {
                _logger.LogInformation(
                    "Cliente desconectado. ConnectionId: {ConnectionId}",
                    Context.ConnectionId
                );
            }

            await base.OnDisconnectedAsync(exception);

        }
    }
}
