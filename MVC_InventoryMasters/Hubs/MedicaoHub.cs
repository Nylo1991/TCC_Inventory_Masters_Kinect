using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Hubs
{
    public class MedicaoHub : Hub
    {
        private readonly MedicaoVolumeRepository _medicaoRepository;

        // Construtor único para injeção de dependência
        public MedicaoHub(MedicaoVolumeRepository medicaoRepository)
        {
            _medicaoRepository = medicaoRepository;
        }

        public async Task EnviarVolume(int quantidadePontos, double escalaEspacial, double fatorCorrecao)
        {
            try
            {
                double volumeCalculado = (quantidadePontos * fatorCorrecao) * escalaEspacial;

                var medicao = new MedicaoVolume
                {
                    OrigemLeitura = "Kinect",
                    Status = "normal",
                    VolumeMedido = volumeCalculado,
                    DataHora = DateTime.UtcNow
                };

                await _medicaoRepository.Adicionar(medicao);

                await Clients.All.SendAsync("NovaMedicao", new
                {
                    volumeMedido = volumeCalculado,
                    pontos = quantidadePontos,
                    dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO no Hub] {ex.Message}");
                throw;
            }
        }

        public override async Task OnConnectedAsync() => await base.OnConnectedAsync();
        public override async Task OnDisconnectedAsync(Exception? exception) => await base.OnDisconnectedAsync(exception);
    }
}