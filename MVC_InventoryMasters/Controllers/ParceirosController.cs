using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;

namespace MVC_InventoryMasters.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento
    /// dos parceiros cadastrados no sistema.
    ///
    /// Fluxo:
    /// View -> Controller -> Repository -> Firestore
    /// </summary>
    public class ParceirosController : Controller
    {
        private readonly ParceirosRepository _repository;

        /// <summary>
        /// Recebe o repositório por Injeção de Dependência.
        /// </summary>
        /// <param name="repository">
        /// Repositório responsável pela comunicação
        /// com a coleção Parceiros do Firestore.
        /// </param>
        public ParceirosController(
            ParceirosRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Exibe a lista de parceiros cadastrados.
        /// </summary>
        /// <returns>
        /// View contendo todos os parceiros.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var parceiros = await _repository.ListarTodos();

            return View(parceiros);
        }

        /// <summary>
        /// Exibe o formulário de cadastro.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Salva um novo parceiro no Firestore.
        /// </summary>
        /// <param name="parceiro">
        /// Dados informados pelo usuário.
        /// </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Parceiro parceiro)
        {
            if (!ModelState.IsValid)
            {
                foreach (var erro in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ERRO: {erro.ErrorMessage}");
                }

                return View(parceiro);
            }

            await _repository.Adicionar(parceiro);

            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Carrega os dados do parceiro para edição.
        /// </summary>
        /// <param name="id">
        /// ID do documento no Firestore.
        /// </param>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var parceiro =
                await _repository.BuscarPorId(id);

            if (parceiro == null)
                return NotFound();

            return View(parceiro);
        }

        /// <summary>
        /// Salva as alterações realizadas no parceiro.
        /// </summary>
        /// <param name="parceiro">
        /// Dados atualizados.
        /// </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Parceiro parceiro)
        {


            if (!ModelState.IsValid)
            {
                return View(parceiro);
            }

            var existente = await _repository.BuscarPorId(parceiro.Id);

            if (existente == null)
                return NotFound();

            string telefoneBanco = new string(
    (existente.Telefone ?? "")
    .Where(char.IsDigit)
    .ToArray());

            string telefoneTela = new string(
                (parceiro.Telefone ?? "")
                .Where(char.IsDigit)
                .ToArray());

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

            // Mantém a data de cadastro
            parceiro.Data_Cadastro = existente.Data_Cadastro;

            await _repository.Atualizar(parceiro);

            TempData["Sucesso"] = "Parceiro atualizado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Exibe a tela de confirmação de exclusão.
        /// </summary>
        /// <param name="id">
        /// ID do parceiro.
        /// </param>
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var parceiro =
                await _repository.BuscarPorId(id);

            if (parceiro == null)
                return NotFound();

            return View(parceiro);
        }

        /// <summary>
        /// Remove definitivamente o parceiro.
        /// </summary>
        /// <param name="id">
        /// ID do documento.
        /// </param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            await _repository.Excluir(id);

            return RedirectToAction(nameof(Index));
        }
    }
}