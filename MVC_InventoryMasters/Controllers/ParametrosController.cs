using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;

namespace MVC_InventoryMasters.Controllers
{
    public class ParametrosController : Controller
    {
        private readonly ParametrosSistemaRepository _repository;

        public ParametrosController(
            ParametrosSistemaRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Tela de Configurações.
        /// </summary>
        public IActionResult Index()
        {
            var model = _repository.Buscar();

            return View(model);
        }

        /// <summary>
        /// Salva os parâmetros.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Salvar(
            ParametrosSistema model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            model.DataAtualizacao =
                DateTime.UtcNow;

            _repository.Salvar(model);

            TempData["Sucesso"] =
                "Configurações atualizadas com sucesso.";

            return RedirectToAction(nameof(Index));
        }
    }
}