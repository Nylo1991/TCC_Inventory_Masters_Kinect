using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.ViewModels;

namespace MVC_InventoryMasters.Controllers
{
    public class DashboardController : Controller
    {
        private readonly MedicaoVolumeRepository _medicaoRepo;
        private readonly NotificacaoRepository _notificacaoRepo;
        private readonly ParceirosRepository _parceirosRepo;
        private readonly ParametrosSistemaRepository _parametrosRepo;

        // O construtor do controller recebe os repositórios via injeção de dependência
        public DashboardController(
            MedicaoVolumeRepository medicaoRepo,
            NotificacaoRepository notificacaoRepo,
            ParceirosRepository parceirosRepo,
            ParametrosSistemaRepository parametrosRepo)
        {
            _medicaoRepo = medicaoRepo;
            _notificacaoRepo = notificacaoRepo;
            _parceirosRepo = parceirosRepo;
            _parametrosRepo = parametrosRepo;
        }

        public async Task<IActionResult> Index()
        {
            var parceiros = await _parceirosRepo.ListarTodos();
            return View(parceiros); // Envia apenas a lista de parceiros, combinando com a View atual
        }
    }
}