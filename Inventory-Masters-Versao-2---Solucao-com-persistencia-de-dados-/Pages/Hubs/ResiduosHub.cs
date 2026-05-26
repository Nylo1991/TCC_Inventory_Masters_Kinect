using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace InventoryMaster.Hubs
{
    public class ResiduosHub : Hub
    {
        // 📦 Volume do Kinect ou sensor
        public async Task EnviarVolume(double volume)
        {
            Console.WriteLine($"[Hub] Volume recebido: {volume:F2} m³ de {Context.ConnectionId}");

            await Clients.All.SendAsync("ReceberVolume", new
            {
                valor = volume,
                tipo = "volume",
                origem = Context.ConnectionId,
                data = DateTime.UtcNow
            });
        }

        // 📡 Status do sistema
        public async Task EnviarStatus(string mensagem)
        {
            Console.WriteLine($"[Hub] Status: {mensagem}");

            await Clients.All.SendAsync("ReceberStatus", new
            {
                mensagem,
                tipo = "status",
                origem = Context.ConnectionId,
                data = DateTime.UtcNow
            });
        }

        // 📊 Evento genérico (MUITO útil para Kinect futuramente)
        public async Task EnviarLeitura(string tipo, double valor)
        {
            await Clients.All.SendAsync("ReceberLeitura", new
            {
                tipo,
                valor,
                origem = Context.ConnectionId,
                data = DateTime.UtcNow
            });
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"[Hub] Cliente conectado: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"[Hub] Cliente desconectado: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}