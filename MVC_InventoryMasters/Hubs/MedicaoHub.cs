using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Hubs
{
    /// <summary>
    /// Hub responsável por enviar medições em tempo real
    /// para o Dashboard via SignalR.
    /// </summary>
    public class MedicaoHub : Hub
    {
        /// <summary>
        /// Recebe o volume enviado pelo cliente (Kinect/WPF) e repassa para todos conectados (Dashboard).
        /// </summary>
        public async Task EnviarVolume(double volume)
        {
            Console.WriteLine($"[Hub] Volume recebido: {volume:F2} cm³ de {Context.ConnectionId}");

            // Repassa o dado para todos os clientes conectados (incluindo o seu Dashboard)
            await Clients.All.SendAsync("ReceberVolume", volume);
        }

        /// <summary>
        /// Recebe mensagens de status do cliente e repassa para todos conectados.
        /// </summary>
        public async Task EnviarStatus(string mensagem)
        {
            Console.WriteLine($"[Hub] Status recebido: '{mensagem}' de {Context.ConnectionId}");

            // Repassa o status para todos os clientes conectados
            await Clients.All.SendAsync("ReceberStatus", mensagem);
        }

        /// <summary>
        /// Loga quando um cliente se conecta.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[Hub] Cliente conectado: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Loga quando um cliente se desconecta.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"[Hub] Cliente desconectado: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}