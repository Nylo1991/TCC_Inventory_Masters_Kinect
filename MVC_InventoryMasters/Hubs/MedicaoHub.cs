using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Hubs
{
    /// <summary>
    /// Hub responsável pela comunicação em tempo real entre
    /// sensores, Kinect, ESP32, serviços externos e o Dashboard.
    ///
    /// Utiliza SignalR para transmitir medições instantaneamente
    /// para todos os clientes conectados.
    /// </summary>
    public class MedicaoHub : Hub
    {

        private readonly MedicaoVolumeRepository _medicaoRepository;

        public MedicaoHub(
            MedicaoVolumeRepository medicaoRepository)
        {
            _medicaoRepository = medicaoRepository;
        }

        /// <summary>
        /// Recebe uma medição de volume enviada por um sensor
        /// e retransmite para todos os clientes conectados.
        /// </summary>
        /// <param name="volume">
        /// Volume calculado pelo sensor ou Kinect.
        /// </param>
        public async Task EnviarVolume(double volume)
        {
            try
            {
                Console.WriteLine(
                    $"[SignalR] Volume recebido: {volume:F2}");

                var medicao = new MedicaoVolume
                {
                    OrigemLeitura = "Kinect",
                    Status = "normal",
                    VolumeMedido = volume,
                    DataHora = DateTime.UtcNow
                };

                // SALVA NO FIRESTORE
                await _medicaoRepository.Adicionar(medicao);

                Console.WriteLine(
                    "[Firestore] Medição salva com sucesso.");

                // ENVIA PARA DASHBOARD
                await Clients.All.SendAsync(
                    "NovaMedicao",
                    new
                    {
                        volumeMedido = volume,
                        origemLeitura = "Kinect",
                        status = "normal",
                        dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[ERRO] {ex}");

                throw;
            }
        }

        /// <summary>
        /// Recebe mensagens de status do sistema e envia
        /// para todos os clientes conectados.
        /// </summary>
        /// <param name="mensagem">
        /// Mensagem de status.
        /// </param>
        public async Task EnviarStatus(string status)
        {
            Console.WriteLine(
                $"[SignalR] Status recebido: {status}");

            await Clients.All.SendAsync("ReceberStatus", new
            {
                status,
                origem = Context.ConnectionId,
                data = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            });
        }

        /// <summary>
        /// Método genérico para envio de leituras futuras
        /// como peso, distância, temperatura ou volume.
        /// </summary>
        /// <param name="tipo">Tipo da leitura.</param>
        /// <param name="valor">Valor da leitura.</param>
        public async Task EnviarLeitura(string tipo, double valor)
        {
            Console.WriteLine(
                $"[SignalR] Leitura recebida: {tipo} = {valor}");

            await Clients.All.SendAsync("ReceberLeitura", new
            {
                tipo,
                valor,
                origem = Context.ConnectionId,
                data = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            });
        }

        /// <summary>
        /// Executado automaticamente quando um cliente
        /// estabelece conexão com o Hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine(
                $"[SignalR] Cliente conectado: {Context.ConnectionId}");

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Executado automaticamente quando um cliente
        /// encerra a conexão com o Hub.
        /// </summary>
        /// <param name="exception">
        /// Exceção gerada durante a desconexão, se houver.
        /// </param>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine(
                $"[SignalR] Cliente desconectado: {Context.ConnectionId}");

            await base.OnDisconnectedAsync(exception);
        }
    }
}