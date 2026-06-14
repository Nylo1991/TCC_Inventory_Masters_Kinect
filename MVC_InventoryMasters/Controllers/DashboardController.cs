using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Controllers
{
    public class DashboardController : Controller
    {
        private readonly MedicaoVolumeRepository _medicaoRepo;
        private readonly NotificacaoRepository _notificacaoRepo;
        private readonly ParceirosRepository _parceirosRepo;
        private readonly ParametrosSistemaRepository _parametrosRepo;
        private readonly UsuariosRepository _usuariosRepo;
        private readonly ILogger<DashboardController> _logger;

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
        /// Exibe a página principal do Dashboard com os indicadores de sistema.
        /// </summary>
        /// <returns>Retorna a view do Dashboard preenchida com os dados do modelo.</returns>
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
                _logger.LogError(ex, "Erro crítico ao carregar os dados do Dashboard.");
                
                return RedirectToAction("Error", "Home");
            }
        }
    }
}