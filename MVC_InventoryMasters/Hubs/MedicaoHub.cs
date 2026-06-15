using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Hubs
{
    public class MedicaoHub : Hub
    {
        /// <summary>
        /// Repositório de medições.
        /// </summary>
        private readonly MedicaoVolumeRepository _medicaoRepository;

        /// <summary>
        /// Repositório de parâmetros do sistema.
        /// </summary>
        private readonly ParametrosSistemaRepository _parametrosRepository;

        /// <summary>
        /// Repositório de notificações.
        /// </summary>
        private readonly NotificacaoRepository _notificacaoRepository;


        /// <summary>
        /// Construtor responsável por receber
        /// os serviços necessários para
        /// processamento das medições.
        ///
        /// Fluxo:
        /// Kinect → Medição → Configuração
        /// → Notificação → Dashboard.
        /// </summary>
        public MedicaoHub(
            MedicaoVolumeRepository medicaoRepository,
            ParametrosSistemaRepository parametrosRepository,
            NotificacaoRepository notificacaoRepository)
        {
            _medicaoRepository = medicaoRepository;
            _parametrosRepository = parametrosRepository;
            _notificacaoRepository = notificacaoRepository;
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