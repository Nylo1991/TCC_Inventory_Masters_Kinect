using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
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

        public async Task<IActionResult> Index()
        {
            var parceiros = await _repository.ListarTodos();
            return View(parceiros);
        }

        /// <summary>
        /// Exibe o formulário de cadastro com Ativo = true por padrão.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            // Instancia o objeto com Ativo = true para marcar o checkbox na View
            var novoParceiro = new Parceiro { Ativo = true };

            return View(novoParceiro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Parceiro parceiro)
        {
            if (!ModelState.IsValid)
            {
                return View(parceiro);
            }

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

            // Lógica de verificação de alterações
            string telefoneBanco = new string((existente.Telefone ?? "").Where(char.IsDigit).ToArray());
            string telefoneTela = new string((parceiro.Telefone ?? "").Where(char.IsDigit).ToArray());

            bool houveAlteracao =
                existente.Nome?.Trim() != parceiro.Nome?.Trim() ||
                existente.Email?.Trim() != parceiro.Email?.Trim() ||
                telefoneBanco != telefoneTela ||
                existente.Empresa?.Trim() != parceiro.Empresa?.Trim() ||
                existente.Endereco?.Trim() != parceiro.Endereco?.Trim() ||
                existente.Ativo != parceiro.Ativo;

            if (!houveAlteracao)
            {
                ViewBag.Aviso = "Nenhuma alteração foi realizada no parceiro.";
                return View(parceiro);
            }

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