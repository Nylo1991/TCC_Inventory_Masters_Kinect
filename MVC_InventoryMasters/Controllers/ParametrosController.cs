using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Controllers
{
    /// <summary>
    /// Controller responsável pelas configurações
    /// gerais do sistema.
    /// </summary>
    public class ParametrosController : Controller
    {
        private readonly ParametrosSistemaRepository _repository;

        public ParametrosController(
            ParametrosSistemaRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Exibe a tela de configurações.
        /// </summary>
        public IActionResult Index()
        {
            var model = _repository.Buscar();

            return View(model);
        }

        /// <summary>
        /// Salva as configurações do sistema.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Salvar(ParametrosSistema model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Index", model);
                }

                model.DataAtualizacao =
                    DateTime.UtcNow;

                _repository.Salvar(model);

                TempData["Sucesso"] =
                    "Configurações atualizadas com sucesso.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Erro"] =
                    $"Erro ao salvar configurações: {ex.Message}";

                return View("Index", model);
            }
        }

    }
}