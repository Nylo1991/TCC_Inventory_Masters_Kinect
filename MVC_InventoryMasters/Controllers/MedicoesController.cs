using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar as ações relacionadas às medições de volume realizadas pelos sensores.
    /// </summary>
    public class MedicoesController : Controller
    {
        private readonly MedicaoVolumeRepository _repo;
        private readonly IHubContext<MedicaoHub> _hub;
        private readonly ILogger<MedicoesController> _logger;

        public MedicoesController(
            MedicaoVolumeRepository repo,
            IHubContext<MedicaoHub> hub,
            ILogger<MedicoesController> logger)
        {
            _repo = repo;
            _hub = hub;
            _logger = logger;
        }

        /// <summary>
        /// Exibe a lista paginada de medições e estatísticas gerais.
        /// </summary>
        public async Task<IActionResult> Index(int pagina = 1)
        {
            try
            {
                const int itensPorPagina = 10;
                var todasMedicoes = await _repo.ListarTodos();

                var listaOrdenada = todasMedicoes
                    .OrderByDescending(x => x.DataHora)
                    .ToList();

                int totalRegistros = listaOrdenada.Count;
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

                // Tratamento de paginação
                pagina = Math.Clamp(pagina, 1, Math.Max(1, totalPaginas));

                var medicoesPaginadas = listaOrdenada
                    .Skip((pagina - 1) * itensPorPagina)
                    .Take(itensPorPagina)
                    .ToList();

                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.PaginaAtual = pagina;
                ViewBag.ItensPorPagina = itensPorPagina;

                ViewBag.VolumeMedio = listaOrdenada.Any() ? listaOrdenada.Average(x => x.VolumeMedido ?? 0) : 0;
                ViewBag.UltimaMedicao = listaOrdenada.Any() ? listaOrdenada.First().DataHora : null;

                return View(medicoesPaginadas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro crítico ao carregar o histórico de medições.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Retorna resumo estatístico em formato JSON.
        /// </summary>
        public async Task<IActionResult> Summary()
        {
            try
            {
                var summary = await _repo.ObterSummary();
                return Json(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar o resumo estatístico.");
                return StatusCode(500, "Erro ao processar os dados estatísticos.");
            }
        }
    }
}