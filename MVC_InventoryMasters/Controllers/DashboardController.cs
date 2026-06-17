using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar as ações relacionadas ao Dashboard do sistema
    /// </summary>
    /// <remarks>
    /// O Dashboard exibe indicadores de desempenho, alertas e informações relevantes para os usuários do sistema.
    /// </remarks>
    /// <param name="DashboardController">Controlador do Dashboard</param>
    /// <retorna>View do Dashboard</returns>
    public class DashboardController : Controller
    {
        private readonly MedicaoVolumeRepository _medicaoRepo;
        private readonly NotificacaoRepository _notificacaoRepo;
        private readonly ParceirosRepository _parceirosRepo;
        private readonly ParametrosSistemaRepository _parametrosRepo;
        private readonly UsuariosRepository _usuariosRepo;
        private readonly ILogger<DashboardController> _logger;

        /// <summary>
        /// Construtor do DashboardController, responsável por injetar as dependências necessárias para o funcionamento do controlador.
        /// </summary>
        /// remarks> As dependências incluem repositórios para acesso a dados de medições, notificações, parceiros, 
        /// parâmetros do sistema e usuários, além de um logger para registro de eventos e erros.</remarks>
        /// <param name="medicaoRepo"></param>
        /// <param name="notificacaoRepo"></param>
        /// <param name="parceirosRepo"></param>
        /// <param name="parametrosRepo"></param>
        /// <param name="usuariosRepo"></param>
        /// <param name="logger"></param>
        /// <returns>Instância do DashboardController com as dependências injetadas</returns>
        public DashboardController(
            MedicaoVolumeRepository medicaoRepo,
            NotificacaoRepository notificacaoRepo,
            ParceirosRepository parceirosRepo,
            ParametrosSistemaRepository parametrosRepo,
            UsuariosRepository usuariosRepo,
            ILogger<DashboardController> logger)
        {
            _medicaoRepo = medicaoRepo;
            _notificacaoRepo = notificacaoRepo;
            _parceirosRepo = parceirosRepo;
            _parametrosRepo = parametrosRepo;
            _usuariosRepo = usuariosRepo;
            _logger = logger;
        }

        /// <summary>
        /// Ação responsável por carregar os dados necessários para exibir o Dashboard, 
        /// incluindo medições, notificações, parceiros, usuários e parâmetros do sistema.
        /// </summary>
        /// <remarks> A ação realiza o cálculo do percentual de ocupação com base na última medição 
        /// e na capacidade máxima definida nos parâmetros do sistema.</remarks>
        /// <param name="Index"></param>
        /// <returns>View do Dashboard com os dados carregados</returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                var parceiros = await _parceirosRepo.ListarTodos();
                var usuarios = await _usuariosRepo.ListarTodos();
                var medicoes = await _medicaoRepo.ListarTodos();
                var alertas = await _notificacaoRepo.ListarTodos();

                var parametros = _parametrosRepo.Buscar();

                var ultimaMedicao = medicoes.OrderByDescending(m => m.DataHora).FirstOrDefault()?.VolumeMedido ?? 0;
              
                double capacidade = parametros.CapacidadeMaxima > 0 ? parametros.CapacidadeMaxima : 10000.0;
              
                decimal percentual = capacidade > 0 ? (decimal)((double)ultimaMedicao / capacidade) * 100 : 0;

                var model = new DashboardViewModel
                {
                    Parceiros = parceiros,
                    Usuarios = usuarios,
                    Medicoes = medicoes,
                    Alertas = alertas,
                    PercentualOcupacao = Math.Min(percentual, 100),
                    Parametros = parametros
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao executar {Controller}.{Action}",
                    nameof(DashboardController),
                    nameof(Index));

                return RedirectToAction("Error", "Home");
            }
        }
    }
}