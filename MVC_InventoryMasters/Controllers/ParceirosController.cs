using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Controllers
{
    public class ParceirosController : Controller
    {
        private readonly ParceirosRepository _repository;

        public ParceirosController(ParceirosRepository repository)
        {
            _repository = repository;
        }

        // AÇÃO INDEX COM PAGINAÇÃO
        public async Task<IActionResult> Index(int pagina = 1)
        {
            int itensPorPagina = 10;
            var todosParceiros = await _repository.ListarTodos();
            int totalRegistros = todosParceiros.Count();

            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

            var parceirosPaginados = todosParceiros
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToList();

            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaAtual = pagina;
            ViewBag.TotalRegistros = totalRegistros;

            return View(parceirosPaginados);
        }

        // AÇÃO DETAILS ADICIONADA
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var parceiro = await _repository.BuscarPorId(id);
            if (parceiro == null) return NotFound();

            return View(parceiro);
        }

        [HttpGet]
        public IActionResult Create() => View(new Parceiro { Ativo = true });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Parceiro parceiro)
        {
            if (!ModelState.IsValid) return View(parceiro);
            await _repository.Adicionar(parceiro);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var parceiro = await _repository.BuscarPorId(id);
            if (parceiro == null) return NotFound();
            return View(parceiro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Parceiro parceiro)
        {
            if (!ModelState.IsValid) return View(parceiro);

            var existente = await _repository.BuscarPorId(parceiro.Id);
            if (existente == null) return NotFound();

            parceiro.Data_Cadastro = existente.Data_Cadastro;
            await _repository.Atualizar(parceiro);

            TempData["Sucesso"] = "Parceiro atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            var parceiro = await _repository.BuscarPorId(id);
            if (parceiro == null) return NotFound();
            return View(parceiro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();
            await _repository.Excluir(id);
            return RedirectToAction(nameof(Index));
        }
    }
}