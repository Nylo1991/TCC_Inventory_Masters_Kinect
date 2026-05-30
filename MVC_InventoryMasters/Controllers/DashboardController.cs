using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Repositories;

namespace MVC_InventoryMasters.Controllers
{    
    public class DashboardController : Controller
    {
        private readonly MedicaoVolumeRepository _medicaoRepo;
        private readonly NotificacaoRepository _notificacaoRepo;
        private readonly ParceirosRepository _parceirosRepo;
        private readonly ParametrosSistemaRepository _parametrosRepo;

        // Injeção dos 4 repositórios no construtor
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

        // Ação para exibir o dashboard, onde vamos buscar os dados
        // de todos os repositórios e enviar para a view
        public IActionResult Index()
        {
            // Cria um objeto "ViewModel" anônimo para enviar tudo de uma vez para a view
            var viewModel = new
            {
                Medicoes = _medicaoRepo.ListarTodas(),
                Alertas = _notificacaoRepo.ListarHistorico(),
                Parceiros = _parceirosRepo.ListarTodos(),
                Parametros = _parametrosRepo.Buscar()
            };

            return View(viewModel);
        }
    }
}