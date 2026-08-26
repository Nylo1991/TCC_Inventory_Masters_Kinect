using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Filters;
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
    [PermissaoAuthorize(PermissoesSistema.ConfiguracoesGerenciar)]
    public class ParametrosController : Controller
    {
        private readonly IParametrosSistemaRepository _repository;
        private readonly ILogger<ParametrosController> _logger;

        public ParametrosController(
            IParametrosSistemaRepository repository,
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
                    atual.DiasSemColetaAlerta != model.DiasSemColetaAlerta ||
                    atual.AtivarSistemaCalibracao != model.AtivarSistemaCalibracao ||
                    atual.RaioDeteccaoKinect != model.RaioDeteccaoKinect ||
                    atual.HabilitarZonaExclusaoDeteccao != model.HabilitarZonaExclusaoDeteccao ||
                    atual.TaxaAmostragemVolumeMinutos != model.TaxaAmostragemVolumeMinutos ||
                    atual.DuracaoMaximaMedicaoSegundos != model.DuracaoMaximaMedicaoSegundos ||
                    atual.TipoAlertaPadrao != model.TipoAlertaPadrao ||
                    atual.TemplateMensagemPadrao != model.TemplateMensagemPadrao ||
                    atual.CanalEmailAtivo != model.CanalEmailAtivo ||
                    atual.CanalWhatsAppAtivo != model.CanalWhatsAppAtivo ||
                    atual.CanalDashboardPushAtivo != model.CanalDashboardPushAtivo ||
                    atual.NomeRemetenteWhatsApp != model.NomeRemetenteWhatsApp ||
                    atual.EscalonamentoMinutos != model.EscalonamentoMinutos ||
                    atual.CanalEscalonamento != model.CanalEscalonamento;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IniciarCalibracao()
        {
            try
            {
                var parametros = _repository.Buscar();
                parametros.AtivarSistemaCalibracao = true;
                parametros.DataAtualizacao = DateTime.UtcNow;

                _repository.Salvar(parametros);

                TempData["Sucesso"] = "Nova calibração do Kinect iniciada com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar a calibração do Kinect.");
                TempData["Erro"] = "Não foi possível iniciar a calibração do Kinect.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RestaurarPadroes()
        {
            try
            {
                var parametros = _repository.ObterPadroes();
                parametros.DataAtualizacao = DateTime.UtcNow;

                _repository.Salvar(parametros);

                TempData["Sucesso"] = "Padrões globais restaurados com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao restaurar os padrões globais dos parâmetros.");
                TempData["Erro"] = "Não foi possível restaurar os padrões globais.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
