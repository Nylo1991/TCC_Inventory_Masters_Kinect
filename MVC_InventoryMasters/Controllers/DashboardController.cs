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
        private readonly UsuariosRepository _usuariosRepo;

        public DashboardController(
            MedicaoVolumeRepository medicaoRepo,
            NotificacaoRepository notificacaoRepo,
            ParceirosRepository parceirosRepo,
            ParametrosSistemaRepository parametrosRepo,
            UsuariosRepository usuariosRepo)
        {
            _medicaoRepo = medicaoRepo;
            _notificacaoRepo = notificacaoRepo;
            _parceirosRepo = parceirosRepo;
            _parametrosRepo = parametrosRepo;
            _usuariosRepo = usuariosRepo;
        }

        public async Task<IActionResult> Index()
        {
            var parceiros = await _parceirosRepo.ListarTodos();
            var usuarios = await _usuariosRepo.ListarTodos();
            var medicoes = await _medicaoRepo.ListarTodos();
           // var alertas = await _notificacaoRepo.ListarTodos();

            var model = new DashboardViewModel
            {
                Parceiros = parceiros,
                Usuarios = usuarios,
                Medicoes = medicoes
                //Alertas = alertas
            };

            return View(model);
        }
    }
}