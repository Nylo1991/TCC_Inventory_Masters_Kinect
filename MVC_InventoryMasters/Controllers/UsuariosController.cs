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
    /// Controller responsável pelo gerenciamento de usuários do sistema.
    /// </summary>
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

        /// <summary>
        /// Lista os usuários com paginação.
        /// </summary>
        /// <param name="pagina">Número da página atual para exibição.</param>
        /// <returns>Retorna a view contendo a lista paginada de usuários.</returns>
        /// <remarks>A paginação é fixa em 10 registros por página.</remarks>
        public async Task<IActionResult> Index(int pagina = 1)
        {
            try
            {
                int itensPorPagina = 10;
                var todosUsuarios = await _repository.ListarTodos();
                int totalRegistros = todosUsuarios.Count();

                var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

                var usuariosPaginados = todosUsuarios
                    .Skip((pagina - 1) * itensPorPagina)
                    .Take(itensPorPagina)
                    .ToList();

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
        /// Exibe a tela de criação de um novo usuário.
        /// </summary>
        /// <returns>Retorna a view de criação carregada com os perfis disponíveis.</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarPerfis();
            return View(new Usuario { Ativo = true });
        }

        /// <summary>
        /// Persiste um novo usuário no banco de dados.
        /// </summary>
        /// <param name="usuario">Objeto contendo os dados do novo usuário.</param>
        /// <returns>Redireciona para a listagem em caso de sucesso ou retorna a view com erros de validação.</returns>
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

        /// <summary>
        /// Exibe a tela de edição de um usuário específico.
        /// </summary>
        /// <param name="id">O identificador único do usuário.</param>
        /// <returns>Retorna a view de edição com os dados do usuário.</returns>
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

        /// <summary>
        /// Atualiza os dados de um usuário existente.
        /// </summary>
        /// <param name="usuario">Dados do usuário submetidos pelo formulário.</param>
        /// <returns>Redireciona para a listagem ou retorna para a edição em caso de erro ou nenhuma alteração.</returns>
        /// <remarks>Realiza a preservação da senha e data de cadastro original.</remarks>
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

        /// <summary>
        /// Exibe a tela de confirmação de exclusão.
        /// </summary>
        /// <param name="id">O identificador único do usuário.</param>
        /// <returns>Retorna a view de confirmação.</returns>
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

        /// <summary>
        /// Confirma a exclusão de um usuário no banco de dados.
        /// </summary>
        /// <param name="id">O identificador do usuário a ser removido.</param>
        /// <returns>Redireciona para o Index após a exclusão.</returns>
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

        /// <summary>
        /// Exibe os detalhes de um usuário.
        /// </summary>
        /// <param name="id">O identificador único do usuário.</param>
        /// <returns>Retorna a view com as informações detalhadas.</returns>
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