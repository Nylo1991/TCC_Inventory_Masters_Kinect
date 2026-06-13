using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MVC_InventoryMasters.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuariosRepository _repository;
        private readonly PerfisRepository _perfisRepository;

        public UsuariosController(
            UsuariosRepository repository,
            PerfisRepository perfisRepository)
        {
            _repository = repository;
            _perfisRepository = perfisRepository;
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

        public async Task<IActionResult> Index(int pagina = 1)
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarPerfis();
            var novoUsuario = new Usuario { Ativo = true };
            return View(novoUsuario);
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

            await _repository.Adicionar(usuario);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var usuario = await _repository.BuscarPorId(id);
            if (usuario == null) return NotFound();

            await CarregarPerfis();
            return View(usuario);
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

            var existente = await _repository.BuscarPorId(usuario.Id);
            if (existente == null) return NotFound();

            // Lógica de verificação de alteração real
            bool houveAlteracao =
                !string.Equals(usuario.Nome?.Trim(), existente.Nome?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(usuario.Email?.Trim(), existente.Email?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(usuario.Perfil?.Trim(), existente.Perfil?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                usuario.Ativo != existente.Ativo;

            if (!houveAlteracao)
            {
                await CarregarPerfis();
                ViewBag.Aviso = "Nenhuma alteração foi realizada. Os dados permanecem os mesmos.";
                return View(usuario);
            }

            // Mantém dados sensíveis e de controle
            usuario.Senha = existente.Senha;
            usuario.Data_Cadastro = existente.Data_Cadastro;

            await _repository.Atualizar(usuario);
            TempData["Sucesso"] = "Usuário atualizado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var usuario = await _repository.BuscarPorId(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            await _repository.Excluir(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var usuario = await _repository.BuscarPorId(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }
    }
}