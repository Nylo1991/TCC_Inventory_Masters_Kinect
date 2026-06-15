using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar as ações relacionadas aos usuários do sistema,
    /// </summary>
    /// <remarks>Este controlador permite listar, criar, editar, excluir e visualizar detalhes dos usuários cadastrados no sistema.</remarks>
    /// <param></param>
    /// <returns></returns>
    public class UsuariosController : Controller
    {
        private readonly UsuariosRepository _repository;
        private readonly PerfisRepository _perfisRepository;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            UsuariosRepository repository,
            PerfisRepository perfisRepository,
            ILogger<UsuariosController> logger)
        {
            _repository = repository;
            _perfisRepository = perfisRepository;
            _logger = logger;
        }

        private async Task CarregarPerfis()
        {
            var perfis = await _perfisRepository.ListarTodos();
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
            bool? ativo = null)
        {
            try
            {
                int itensPorPagina = 10;

                var todosUsuarios = await _repository.ListarTodos();

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    todosUsuarios = todosUsuarios
                        .Where(u =>
                            (!string.IsNullOrEmpty(u.Nome) &&
                             u.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(u.Email) &&
                             u.Email.Contains(termo, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(perfil))
                {
                    todosUsuarios = todosUsuarios
                        .Where(u =>
                            !string.IsNullOrEmpty(u.Perfil) &&
                            u.Perfil.Contains(perfil, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (ativo.HasValue)
                {
                    todosUsuarios = todosUsuarios
                        .Where(u => u.Ativo == ativo.Value)
                        .ToList();
                }

                int totalRegistros = todosUsuarios.Count();

                var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

                var usuariosPaginados = todosUsuarios
                    .Skip((pagina - 1) * itensPorPagina)
                    .Take(itensPorPagina)
                    .ToList();

                ViewBag.Termo = termo;
                ViewBag.Perfil = perfil;
                ViewBag.Ativo = ativo;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.PaginaAtual = pagina;
                ViewBag.TotalRegistros = totalRegistros;

                return View(usuariosPaginados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar listagem de usuários.");
                return RedirectToAction("Error", "Home");
            }
        }
        /// <summary>
        /// Exibe a tela de criação de um novo usuário, carregando os perfis disponíveis para seleção.
        /// </summary>
        /// <remarks
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarPerfis();
            return View(new Usuario { Ativo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                await CarregarPerfis();
                return View(usuario);
            }

            try
            {
                await _repository.Adicionar(usuario);
                TempData["Sucesso"] = "Usuário cadastrado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar novo usuário.");
                TempData["Erro"] = "Ocorreu um erro ao salvar o usuário.";
                await CarregarPerfis();
                return View(usuario);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                var usuario = await _repository.BuscarPorId(id);
                if (usuario == null) return NotFound();

                await CarregarPerfis();
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar tela de edição do usuário ID: {Id}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Usuario usuario)
        {
            ModelState.Remove(nameof(usuario.Senha));

            if (!ModelState.IsValid)
            {
                await CarregarPerfis();
                return View(usuario);
            }

            try
            {
                var existente = await _repository.BuscarPorId(usuario.Id);
                if (existente == null) return NotFound();

                bool houveAlteracao =
                    !string.Equals(usuario.Nome?.Trim(), existente.Nome?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(usuario.Email?.Trim(), existente.Email?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(usuario.Perfil?.Trim(), existente.Perfil?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    usuario.Ativo != existente.Ativo;

                if (!houveAlteracao)
                {
                    await CarregarPerfis();
                    ViewBag.Aviso = "Nenhuma alteração foi realizada.";
                    return View(usuario);
                }

                usuario.Senha = existente.Senha;
                usuario.Data_Cadastro = existente.Data_Cadastro;

                await _repository.Atualizar(usuario);
                TempData["Sucesso"] = "Usuário atualizado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário ID: {Id}", usuario.Id);
                TempData["Erro"] = "Ocorreu um erro ao atualizar os dados.";
                await CarregarPerfis();
                return View(usuario);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                var usuario = await _repository.BuscarPorId(id);
                return usuario == null ? NotFound() : View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar tela de exclusão do usuário ID: {Id}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                await _repository.Excluir(id);
                TempData["Sucesso"] = "Usuário excluído com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário ID: {Id}", id);
                TempData["Erro"] = "Não foi possível excluir o usuário.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                var usuario = await _repository.BuscarPorId(id);
                return usuario == null ? NotFound() : View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar detalhes do usuário ID: {Id}", id);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}