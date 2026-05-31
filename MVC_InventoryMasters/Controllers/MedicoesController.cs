using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;

namespace MVC_InventoryMasters.Controllers
{
    /// <summary>
    /// Controller MVC responsável pela tela de Medições.
    /// Exibe dados no dashboard e permite consulta histórica.
    /// </summary>
    public class MedicoesController : Controller
    {
        private readonly MedicaoVolumeRepository _repo;
        private readonly IHubContext<MedicaoHub> _hub;

        /// <summary>
        /// Construtor com injeção de dependência.
        /// </summary>
        public MedicoesController(
            MedicaoVolumeRepository repo,
            IHubContext<MedicaoHub> hub)
        {
            _repo = repo;
            _hub = hub;
        }

        /// <summary>
        /// Exibe a lista de medições na interface.
        /// Utilizado para histórico e visualização administrativa.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var medicoes = await _repo.ListarTodos();
            return View(medicoes);
        }

        /// <summary>
        /// Retorna um resumo estatístico das medições.
        /// Usado para KPIs e dashboards analíticos.
        /// </summary>
        public async Task<IActionResult> Summary()
        {
            var medicoes = await _repo.ListarTodos();

            var summary = new
            {
                Total = medicoes.Count,
                Media = medicoes.Any() ? medicoes.Average(m => m.VolumeMedido ?? 0) : 0,
                Maximo = medicoes.Any() ? medicoes.Max(m => m.VolumeMedido ?? 0) : 0,
                Minimo = medicoes.Any() ? medicoes.Min(m => m.VolumeMedido ?? 0) : 0
            };

            return Json(summary);
        }
    }
}