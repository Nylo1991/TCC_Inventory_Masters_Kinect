using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Models;

namespace MVC_InventoryMasters.Hubs
{
    public class MedicaoHub : Hub
    {
        // Kinect envia para cá
        public async Task EnviarMedicao(MedicaoVolume medicao)
        {
            await Clients.All.SendAsync("NovaMedicao", medicao);
        }
    }
}