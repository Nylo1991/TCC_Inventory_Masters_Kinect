using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Filters;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Controllers
{
    [PermissaoAuthorize(PermissoesSistema.PerfisGerenciar)]
    public class PerfisController : Controller
    {
        private readonly IPerfisRepository _repository;
        private readonly ILogger<PerfisController> _logger;

        public PerfisController(IPerfisRepository repository, ILogger<PerfisController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pagina = 1, string termo = null, bool? ativo = null)
        {
            try
            {
                const int itensPorPagina = 10;
                var perfis = await _repository.ListarPorEmpresa();

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    perfis = perfis.Where(p =>
                        (p.Nome ?? "").Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                        (p.Descricao ?? "").Contains(termo, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (ativo.HasValue)
                    perfis = perfis.Where(p => p.Ativo == ativo.Value).ToList();

                int totalRegistros = perfis.Count;
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);
                pagina = Math.Clamp(pagina, 1, Math.Max(1, totalPaginas));

                ViewBag.Termo = termo;
                ViewBag.Ativo = ativo;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.PaginaAtual = pagina;

                return View(perfis.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar perfis.");
                TempData["Erro"] = "Não foi possível carregar os perfis.";
                return View(new List<Perfil>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Permissoes = PermissoesSistema.Todas;
            return View(new Perfil { Ativo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Perfil perfil, string[] permissoesSelecionadas)
        {
            try
            {
                perfil.Permissoes = permissoesSelecionadas?.ToList() ?? new List<string>();
                await _repository.Adicionar(perfil);
                TempData["Sucesso"] = "Perfil cadastrado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar perfil.");
                ViewBag.Permissoes = PermissoesSistema.Todas;
                TempData["Erro"] = "Não foi possível cadastrar o perfil.";
                return View(perfil);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var perfil = await _repository.BuscarPorId(id);
            if (perfil == null) return NotFound();

            ViewBag.Permissoes = PermissoesSistema.Todas;
            return View(perfil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Perfil perfil, string[] permissoesSelecionadas)
        {
            try
            {
                perfil.Permissoes = permissoesSelecionadas?.ToList() ?? new List<string>();
                await _repository.Atualizar(perfil);
                TempData["Sucesso"] = "Perfil atualizado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar perfil {PerfilId}.", perfil.Id);
                ViewBag.Permissoes = PermissoesSistema.Todas;
                TempData["Erro"] = "Não foi possível atualizar o perfil.";
                return View(perfil);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var perfil = await _repository.BuscarPorId(id);
            return perfil == null ? NotFound() : View(perfil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inativar(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            await _repository.Inativar(id);
            TempData["Sucesso"] = "Perfil inativado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}
