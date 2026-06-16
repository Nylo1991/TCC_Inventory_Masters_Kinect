using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;

namespace MVC_InventoryMasters.Hubs
{
    /// <summary>
    /// Hub responsável por receber as medições enviadas
    /// pelo Kinect, armazenar no Firestore e distribuir
    /// as atualizações em tempo real para o Dashboard.
    /// </summary>
    public class MedicaoHub : Hub
    {
        private readonly MedicaoVolumeRepository _medicaoRepository;
        private readonly ParametrosSistemaRepository _parametrosRepository;
        private readonly NotificacaoRepository _notificacaoRepository;

        /// <summary>
        /// Inicializa o Hub de medições.
        /// </summary>
        public MedicaoHub(
            MedicaoVolumeRepository medicaoRepository,
            ParametrosSistemaRepository parametrosRepository,
            NotificacaoRepository notificacaoRepository)
        {
            _medicaoRepository = medicaoRepository;
            _parametrosRepository = parametrosRepository;
            _notificacaoRepository = notificacaoRepository;
        }

        /// <summary>
        /// Recebe o volume calculado pelo Kinect,
        /// salva a medição e atualiza os clientes
        /// conectados em tempo real.
        /// </summary>
        /// <param name="volumeCm3">
        /// Volume recebido do Kinect em cm³.
        /// </param>
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
                Console.WriteLine(
                    $"[ERRO MedicaoHub] {ex}");
                throw;
            }
        }

        /// <summary>
        /// Verifica se o volume atual atingiu
        /// os limites configurados e gera
        /// notificações automáticas.
        /// </summary>
        private async Task VerificarAlertas(
            double volumeAtual)
        {
            try
            {
                var parametros =
                    _parametrosRepository.Buscar();

                if (parametros == null)
                    return;

                double percentual =
                    (volumeAtual /
                     parametros.CapacidadeMaxima) * 100;

                if (percentual <
                    parametros.PercentualAlerta)
                {
                    return;
                }

                bool existePendente =
                    await _notificacaoRepository
                        .ExisteNotificacaoPendente();

                if (existePendente)
                    return;

                var notificacao =
                    new Notificacao
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[ERRO ALERTA] {ex.Message}");
            }
        }

        /// <summary>
        /// Executado quando um cliente estabelece
        /// conexão com o Hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Executado quando um cliente encerra
        /// a conexão com o Hub.
        /// </summary>
        public override async Task OnDisconnectedAsync(
            Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}