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

        public async Task<IActionResult> Index(
            int pagina = 1,
            string termo = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            bool? ativo = null)
        {
            int itensPorPagina = 10;

            // 1. Obtém a lista filtrada pelo repositório
            var listaFiltrada = await _repository.FiltrarAvancado(termo, dataInicio, dataFim, ativo);

            // 2. Paginação baseada no resultado do filtro
            int totalRegistros = listaFiltrada.Count();
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

            var parceirosPaginados = listaFiltrada
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToList();

            // 3. Passa os filtros para a ViewBag (para o formulário lembrar o que foi pesquisado)
            ViewBag.Termo = termo;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");
            ViewBag.Ativo = ativo;

            // 4. Dados da paginação
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.PaginaAtual = pagina;
            ViewBag.TotalRegistros = totalRegistros;

            return View(parceirosPaginados);
        }

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
            TempData["Sucesso"] = "Parceiro cadastrado com sucesso.";
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

            // Normalização de telefone para comparação segura
            var telExistente = new string((existente.Telefone ?? "").Where(char.IsDigit).ToArray());
            var telFormulario = new string((parceiro.Telefone ?? "").Where(char.IsDigit).ToArray());

            bool houveAlteracao =
                !string.Equals(parceiro.Nome?.Trim(), existente.Nome?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(parceiro.Email?.Trim(), existente.Email?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                telExistente != telFormulario ||
                !string.Equals(parceiro.Empresa?.Trim(), existente.Empresa?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(parceiro.Endereco?.Trim(), existente.Endereco?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                parceiro.Ativo != existente.Ativo;

            if (!houveAlteracao)
            {
                ViewBag.Aviso = "Nenhuma alteração foi realizada. Os dados permanecem os mesmos.";
                return View(parceiro);
            }

            // Preserva dados de controle
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
            TempData["Sucesso"] = "Parceiro excluído com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}