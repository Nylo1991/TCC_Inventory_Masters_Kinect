using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
=======
>>>>>>> 69278f70785abed625eb15930bd6564a7fd280ec
using MVC_InventoryMasters.Filters;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Controllers
{
<<<<<<< HEAD
    /// <summary>
    /// Controlador responsável por gerenciar as ações relacionadas aos usuários do sistema,
    /// </summary>
    /// <remarks>Este controlador permite listar, criar, editar, excluir e visualizar detalhes dos usuários cadastrados no sistema.</remarks>
    /// <param></param>
    /// <returns></returns>
    [PermissaoAuthorize(PermissoesSistema.UsuariosGerenciar)]
    public class UsuariosController : Controller
=======
    [PermissaoAuthorize(PermissoesSistema.PerfisGerenciar)]
    public class PerfisController : Controller
>>>>>>> 69278f70785abed625eb15930bd6564a7fd280ec
    {
        private readonly PerfisRepository _repository;
        private readonly ILogger<PerfisController> _logger;

        public PerfisController(PerfisRepository repository, ILogger<PerfisController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

<<<<<<< HEAD
        private async Task CarregarPerfis()
        {
            var perfis = await _perfisRepository.ListarPorEmpresa();
            ViewBag.Perfis = perfis
                .Select(p => new SelectListItem
                {
                    Value = p.Nome,
                    Text = p.Nome
                })
                .ToList();
        }

        public async Task<IActionResult> Index(
            int pagina = 1,
            string termo = null,
            string perfil = null,
            string empresa = null,
            bool? ativo = null)
        {
            try
            {
                int itensPorPagina = 10;

                var todosUsuarios = await _repository.ListarPorEmpresa();
=======
        public async Task<IActionResult> Index(int pagina = 1, string termo = null, bool? ativo = null)
        {
            try
            {
                const int itensPorPagina = 10;
                var perfis = await _repository.ListarPorEmpresa();
>>>>>>> 69278f70785abed625eb15930bd6564a7fd280ec

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    perfis = perfis.Where(p =>
                        (p.Nome ?? "").Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                        (p.Descricao ?? "").Contains(termo, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(empresa))
                {
                    todosUsuarios = todosUsuarios
                        .Where(u =>
                            (!string.IsNullOrEmpty(u.Empresa) &&
                             u.Empresa.Contains(empresa, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(u.EmpresaId) &&
                             u.EmpresaId.Contains(empresa, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                if (ativo.HasValue)
                    perfis = perfis.Where(p => p.Ativo == ativo.Value).ToList();

                int totalRegistros = perfis.Count;
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);
                pagina = Math.Clamp(pagina, 1, Math.Max(1, totalPaginas));

                ViewBag.Termo = termo;
<<<<<<< HEAD
                ViewBag.Perfil = perfil;
                ViewBag.Empresa = empresa;
=======
>>>>>>> 69278f70785abed625eb15930bd6564a7fd280ec
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
<<<<<<< HEAD
                var existente = await _repository.BuscarPorId(usuario.Id);
                if (existente == null) return NotFound();

                bool houveAlteracao =
                    !string.Equals(usuario.Nome?.Trim(), existente.Nome?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(usuario.Email?.Trim(), existente.Email?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(usuario.Perfil?.Trim(), existente.Perfil?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(usuario.Empresa?.Trim(), existente.Empresa?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    usuario.Ativo != existente.Ativo;

                if (!houveAlteracao)
                {
                    await CarregarPerfis();
                    ViewBag.Aviso = "Nenhuma alteração foi realizada.";
                    return View(usuario);
                }

                usuario.Senha = existente.Senha;
                usuario.Data_Cadastro = existente.Data_Cadastro;
                usuario.EmpresaId = existente.EmpresaId;

                await _repository.Atualizar(usuario);
                TempData["Sucesso"] = "Usuário atualizado com sucesso.";
=======
                perfil.Permissoes = permissoesSelecionadas?.ToList() ?? new List<string>();
                await _repository.Atualizar(perfil);
                TempData["Sucesso"] = "Perfil atualizado com sucesso.";
>>>>>>> 69278f70785abed625eb15930bd6564a7fd280ec
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlternarStatus(string id, bool ativo)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                await _repository.AtualizarStatus(id, !ativo);
                TempData["Sucesso"] = !ativo ? "Usuário ativado com sucesso." : "Usuário inativado com sucesso.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alternar status do usuário ID: {Id}", id);
                TempData["Erro"] = "Não foi possível alterar o status do usuário.";
            }

            return RedirectToAction(nameof(Index));
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
