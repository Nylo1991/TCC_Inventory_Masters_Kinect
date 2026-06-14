using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System;

namespace MVC_InventoryMasters.Controllers
{
    /// <summary>
    /// Controller responsável pelas configurações gerais do sistema.
    /// </summary>
    public class ParametrosController : Controller
    {
        private readonly ParametrosSistemaRepository _repository;
        private readonly ILogger<ParametrosController> _logger;

        public ParametrosController(
            ParametrosSistemaRepository repository,
            ILogger<ParametrosController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Exibe a tela de configurações.
        /// </summary>
        public IActionResult Index()
        {
            try
            {
                var model = _repository.Buscar();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar a tela de parâmetros.");
                TempData["Erro"] = "Ocorreu um erro ao carregar as configurações.";
                return View(new ParametrosSistema());
            }
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
        
                if (model.CapacidadeMinima >= model.CapacidadeMaxima)
                {
                    ModelState.AddModelError(
                        nameof(model.CapacidadeMinima),
                        "A capacidade mínima deve ser menor que a capacidade máxima.");

                    return View("Index", model);
                }

                var atual = _repository.Buscar();

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
                    TempData["Aviso"] = "Nenhuma alteração foi realizada.";
                    return RedirectToAction(nameof(Index));
                }

                model.DataAtualizacao = DateTime.UtcNow;

                _repository.Salvar(model);

                TempData["Sucesso"] = "Configurações atualizadas com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex, "Erro crítico ao tentar salvar os parâmetros do sistema.");
                
                TempData["Erro"] = "Erro interno ao salvar configurações. Tente novamente mais tarde.";

                return View("Index", model);
            }
        }
    }
}