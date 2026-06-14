using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Controllers
{
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

                int totalPaginas = (int)Math.Ceiling(
                    totalRegistros / (double)itensPorPagina);

                if (totalPaginas < 1)
                {
                    totalPaginas = 1;
                }

                if (pagina < 1)
                {
                    pagina = 1;
                }

                if (pagina > totalPaginas)
                {
                    pagina = totalPaginas;
                }

                var medicoesPaginadas = listaOrdenada
                    .Skip((pagina - 1) * itensPorPagina)
                    .Take(itensPorPagina)
                    .ToList();

                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.PaginaAtual = pagina;
                ViewBag.ItensPorPagina = itensPorPagina;

                ViewBag.VolumeMedio = listaOrdenada.Any()
                    ? listaOrdenada.Average(x => x.VolumeMedido ?? 0)
                    : 0;

                ViewBag.UltimaMedicao = listaOrdenada.Any()
                    ? listaOrdenada.First().DataHora
                    : null;

                return View(medicoesPaginadas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro crítico ao carregar o histórico de medições.");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Summary()
        {
            try
            {
                var medicoes = await _repo.ListarTodos();

                var summary = new
                {
                    Total = medicoes.Count,
                    Media = medicoes.Any()
                        ? medicoes.Average(m => m.VolumeMedido ?? 0)
                        : 0,
                    Maximo = medicoes.Any()
                        ? medicoes.Max(m => m.VolumeMedido ?? 0)
                        : 0,
                    Minimo = medicoes.Any()
                        ? medicoes.Min(m => m.VolumeMedido ?? 0)
                        : 0
                };

                return Json(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar o resumo estatístico das medições.");
                return StatusCode(500, "Erro ao processar os dados estatísticos.");
            }
        }
    }
}