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
        private readonly MedicaoVolumeRepository _medicaoRepository;
        private readonly ParametrosSistemaRepository _parametrosRepository;
        private readonly NotificacaoRepository _notificacaoRepository;
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
            MedicaoVolumeRepository medicaoRepository,
            ParametrosSistemaRepository parametrosRepository,
            NotificacaoRepository notificacaoRepository,
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
        public async Task EnviarVolume(double volumeCm3)
        {
            try
            {
                double volumeM3 = volumeCm3 / 1000000d;

                var medicao = new MedicaoVolume
                {
                    OrigemLeitura = "Kinect",
                    Status = "Normal",
                    VolumeMedido = volumeM3,
                    DataHora = DateTime.UtcNow
                };

                await _medicaoRepository.Adicionar(medicao);

                await VerificarAlertas(volumeM3);

                await Clients.All.SendAsync(
                    "NovaMedicao",
                    new
                    {
                        volumeMedido = volumeM3,
                        dataHora = DateTime.UtcNow
                            .ToLocalTime()
                            .ToString("dd/MM/yyyy HH:mm:ss")
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao processar uma medição recebida pelo Kinect."
                );

                await Clients.Caller.SendAsync(
                    "ErroProcessamento",
                    "Não foi possível processar a medição enviada."
                );
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
        private async Task VerificarAlertas(double volumeAtual)
        {
            try
            {
                var parametros = _parametrosRepository.Buscar();

                if (parametros == null)
                {
                    _logger.LogWarning(
                        "Os parâmetros do sistema não foram encontrados."
                    );

                    return;
                }

                if (parametros.CapacidadeMaxima <= 0)
                {
                    _logger.LogWarning(
                        "Capacidade máxima inválida para geração de alertas."
                    );

                    return;
                }

                double percentual =
                    (volumeAtual / parametros.CapacidadeMaxima) * 100;

                if (percentual < parametros.PercentualAlerta)
                {
                    return;
                }

                bool existePendente =
                    await _notificacaoRepository
                        .ExisteNotificacaoPendente();

                if (existePendente)
                {
                    return;
                }

                var notificacao = new Notificacao
                {
                    VolumeMedido = volumeAtual,
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
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao verificar alertas automáticos."
                );
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