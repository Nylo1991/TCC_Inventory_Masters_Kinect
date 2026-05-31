using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Repositories;

namespace MVC_InventoryMasters.Controllers
{
    /// <summary>
    /// Controller MVC responsável pela tela de Medições.
    /// </summary>
    public class MedicoesController : Controller
    {
        private readonly MedicaoVolumeRepository _repo;

        public MedicoesController(MedicaoVolumeRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Exibe a lista de medições na interface.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var medicoes = await _repo.ListarTodos();
            return View(medicoes);
        }
    }
}