using Microsoft.AspNetCore.SignalR;

namespace MVC_InventoryMasters.Hubs
{
    public class NotificacaoHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Console.WriteLine($"Cliente conectado: {Context.ConnectionId}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"Cliente desconectado: {Context.ConnectionId}");
        }

        /// <summary>
        /// Envia uma notificação para todos os clientes conectados.
        /// </summary>
        /// <param name="mensagem">A mensagem de notificação a ser enviada.</param>
        public async Task EnviarNotificacao(string mensagem)
        {
            await Clients.All.SendAsync("ReceberNotificacao", mensagem);
        }
    }
}
