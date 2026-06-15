using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System;

namespace MVC_InventoryMasters.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar as ações relacionadas aos parâmetros do sistema,
    /// </summary>
    /// remarks>Este controlador permite exibir a tela de configurações, validar os dados de entrada e 
    /// salvar as alterações realizadas pelo usuário.</remarks>
    /// <param></param>
    /// <retuns></retuns>
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
        /// Salva as alterações realizadas pelo usuário nos parâmetros do sistema, realizando validações e tratamento de erros.
        /// </summary>
        /// <remarks> Antes de salvar, o método verifica se os dados são válidos e se houve alguma alteração 
        /// em relação aos valores atuais.</remarks>
        /// <param name="model"></param>
        /// <returns></returns>
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