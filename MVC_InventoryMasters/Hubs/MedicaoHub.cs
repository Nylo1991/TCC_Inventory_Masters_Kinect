using Microsoft.AspNetCore.SignalR;
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
        /// Recebe uma medição de volume enviada por um sensor
        /// e retransmite para todos os clientes conectados.
        /// </summary>
        /// <param name="volume">
        /// Volume calculado pelo sensor ou Kinect.
        /// </param>
        public async Task EnviarVolume(double volume)
        {
            Console.WriteLine(
                $"[SignalR] Volume recebido: {volume:F2} m³ | Cliente: {Context.ConnectionId}");

            await Clients.All.SendAsync("NovaMedicao", new
            {
                volumeMedido = volume,
                origemLeitura = Context.ConnectionId,
                status = "Normal",
                dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            });
        }

        /// <summary>
        /// Recebe mensagens de status do sistema e envia
        /// para todos os clientes conectados.
        /// </summary>
        /// <param name="mensagem">
        /// Mensagem de status.
        /// </param>
        public async Task EnviarStatus(string mensagem)
        {
            Console.WriteLine(
                $"[SignalR] Status recebido: {mensagem}");

            await Clients.All.SendAsync("ReceberStatus", new
            {
                mensagem,
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