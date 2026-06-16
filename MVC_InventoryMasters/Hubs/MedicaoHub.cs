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

        /// <summary>
        /// Inicializa o Hub de medições.
        /// </summary>
        /// <param name="medicaoRepository">
        /// Repositório responsável pelo armazenamento
        /// das medições de volume.
        /// </param>
        public MedicaoHub(
            MedicaoVolumeRepository medicaoRepository)
        {
            _medicaoRepository = medicaoRepository;
        }

        /// <summary>
        /// Recebe o volume calculado pelo Kinect,
        /// salva a medição e atualiza os clientes
        /// conectados em tempo real.
        /// </summary>
        /// <param name="volumeCm3">
        /// Volume calculado pelo Kinect em cm³.
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
        /// <param name="exception">
        /// Exceção gerada durante a desconexão,
        /// quando existir.
        /// </param>
        public override async Task OnDisconnectedAsync(
            Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}