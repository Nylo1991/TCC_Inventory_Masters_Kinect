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

        public IActionResult Index()
        {
            // Cria um objeto do tipo DashboardViewModel e
            // preenche suas propriedades com os dados dos repositórios
            var model = new DashboardViewModel
            {
                Medicoes = _medicaoRepo.ListarTodas(),
                Alertas = _notificacaoRepo.ListarHistorico(),
                Parceiros = _parceirosRepo.ListarTodos(),
                Parametros = _parametrosRepo.Buscar()
            };

            return View(model); // Envia o objeto tipado para a View
        }
    }
}