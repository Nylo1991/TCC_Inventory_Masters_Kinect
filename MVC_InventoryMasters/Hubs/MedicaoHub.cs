using Microsoft.AspNetCore.SignalR;

namespace MVC_InventoryMasters.Hubs
{
    /// <summary>
    /// Hub responsável por enviar medições em tempo real
    /// para o Dashboard via SignalR.
    /// </summary>
    public class MedicaoHub : Hub
    {
        /// <summary>
        /// Método chamado quando o cliente conecta (opcional).
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}