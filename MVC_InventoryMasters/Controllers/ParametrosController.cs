using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;

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

                // Regra de negócio:
                // Mínimo deve ser menor que Máximo
                if (model.CapacidadeMinima >= model.CapacidadeMaxima)
                {
                    ModelState.AddModelError(
                        nameof(model.CapacidadeMinima),
                        "A capacidade mínima deve ser menor que a capacidade máxima.");

                    return View("Index", model);
                }

                // Busca configuração atual
                var atual = _repository.Buscar();

                // Verifica se houve alteração
                bool houveAlteracao =
                    atual.CapacidadeMaxima != model.CapacidadeMaxima ||
                    atual.CapacidadeMinima != model.CapacidadeMinima ||
                    atual.PercentualAlerta != model.PercentualAlerta ||
                    atual.NotificacaoAutomatica != model.NotificacaoAutomatica ||
                    atual.ExibirAlertaDashboard != model.ExibirAlertaDashboard ||
                    atual.ParceiroPadraoId != model.ParceiroPadraoId ||
                    atual.DiasSemColetaAlerta != model.DiasSemColetaAlerta;

                if (!houveAlteracao)
                {
                    TempData["Aviso"] =
                        "Nenhuma alteração foi realizada.";

                    return RedirectToAction(nameof(Index));
                }

                model.DataAtualizacao = DateTime.UtcNow;

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