using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace InventoryMaster.Hubs
{
    public class ResiduosHub : Hub
    {
        /// <summary>
        /// Recebe o volume enviado pelo cliente (ex: WPF) e repassa para todos conectados.
        /// </summary>
        public async Task EnviarVolume(double volume)
        {
            Console.WriteLine($"[Hub] Volume recebido: {volume:F2} m³ de {Context.ConnectionId}");
            await Clients.All.SendAsync("ReceberVolume", volume);
        }

        /// <summary>
        /// Recebe mensagens de status do cliente e repassa para todos conectados.
        /// </summary>
        public async Task EnviarStatus(string mensagem)
        {
            Console.WriteLine($"[Hub] Status recebido: '{mensagem}' de {Context.ConnectionId}");
            await Clients.All.SendAsync("ReceberStatus", mensagem);
        }

        /// <summary>
        /// Loga quando um cliente se conecta.
        /// </summary>
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"[Hub] Cliente conectado: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        /// <summary>
        /// Loga quando um cliente se desconecta.
        /// </summary>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"[Hub] Cliente desconectado: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}