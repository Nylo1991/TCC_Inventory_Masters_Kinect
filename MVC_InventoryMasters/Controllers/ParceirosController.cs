using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento dos parceiros do sistema.
    /// </summary>
    public class ParceirosController : Controller
    {
        private readonly ParceirosRepository _repository;
        private readonly ILogger<ParceirosController> _logger;

        public ParceirosController(ParceirosRepository repository, ILogger<ParceirosController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Lista os parceiros com suporte a filtros e paginação.
        /// </summary>
        /// <param name="pagina">Número da página atual.</param>
        /// <param name="termo">Termo para busca por nome ou empresa.</param>
        /// <param name="dataInicio">Data de início para filtro de cadastro.</param>
        /// <param name="dataFim">Data de fim para filtro de cadastro.</param>
        /// <param name="ativo">Filtro por status do parceiro.</param>
        /// <returns>Retorna a view com a lista paginada de parceiros.</returns>
        /// <remarks>A paginação é definida com 10 itens por página.</remarks>
        public async Task<IActionResult> Index(int pagina = 1, string termo = null, DateTime? dataInicio = null, DateTime? dataFim = null, bool? ativo = null)
        {
            try
            {
                int itensPorPagina = 10;
                var listaFiltrada = await _repository.FiltrarAvancado(termo, dataInicio, dataFim, ativo);

                int totalRegistros = listaFiltrada.Count();
                var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);

                var parceirosPaginados = listaFiltrada
                    .Skip((pagina - 1) * itensPorPagina)
                    .Take(itensPorPagina)
                    .ToList();

                ViewBag.Termo = termo;
                ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
                ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");
                ViewBag.Ativo = ativo;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.PaginaAtual = pagina;
                ViewBag.TotalRegistros = totalRegistros;

                return View(parceirosPaginados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar a listagem de parceiros.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Exibe os detalhes de um parceiro específico.
        /// </summary>
        /// <param name="id">O identificador único do parceiro.</param>
        /// <returns>Retorna a view com os dados do parceiro.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                var parceiro = await _repository.BuscarPorId(id);
                if (parceiro == null) return NotFound();
                return View(parceiro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar detalhes do parceiro ID: {Id}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Exibe a tela de criação de um novo parceiro.
        /// </summary>
        /// <returns>Retorna a view com um novo objeto parceiro.</returns>
        [HttpGet]
        public IActionResult Create() => View(new Parceiro { Ativo = true });

        /// <summary>
        /// Persiste um novo parceiro no banco de dados.
        /// </summary>
        /// <param name="parceiro">Objeto com os dados do parceiro.</param>
        /// <returns>Redireciona para o Index em caso de sucesso ou retorna a view com erro.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Parceiro parceiro)
        {
            if (!ModelState.IsValid) return View(parceiro);

            try
            {
                await _repository.Adicionar(parceiro);
                TempData["Sucesso"] = "Parceiro cadastrado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar novo parceiro.");
                TempData["Erro"] = "Ocorreu um erro ao salvar o parceiro.";
                return View(parceiro);
            }
        }

        /// <summary>
        /// Exibe a tela de edição de um parceiro existente.
        /// </summary>
        /// <param name="id">O identificador do parceiro.</param>
        /// <returns>Retorna a view com os dados do parceiro para edição.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                var parceiro = await _repository.BuscarPorId(id);
                if (parceiro == null) return NotFound();
                return View(parceiro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar tela de edição do parceiro ID: {Id}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Atualiza os dados de um parceiro após validar alterações.
        /// </summary>
        /// <param name="parceiro">Dados do parceiro submetidos pelo formulário.</param>
        /// <returns>Redireciona para o Index em caso de sucesso.</returns>
        /// <remarks>Realiza normalização de campos como telefone para detectar mudanças reais.</remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Parceiro parceiro)
        {
            if (!ModelState.IsValid) return View(parceiro);

            try
            {
                var existente = await _repository.BuscarPorId(parceiro.Id);
                if (existente == null) return NotFound();

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
                    ViewBag.Aviso = "Nenhuma alteração foi realizada.";
                    return View(parceiro);
                }

                parceiro.Data_Cadastro = existente.Data_Cadastro;
                await _repository.Atualizar(parceiro);
                TempData["Sucesso"] = "Parceiro atualizado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar parceiro ID: {Id}", parceiro.Id);
                TempData["Erro"] = "Ocorreu um erro ao atualizar os dados.";
                return View(parceiro);
            }
        }

        /// <summary>
        /// Exibe a tela de confirmação para exclusão de um parceiro.
        /// </summary>
        /// <param name="id">O identificador do parceiro.</param>
        /// <returns>Retorna a view de confirmação de exclusão.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                var parceiro = await _repository.BuscarPorId(id);
                return parceiro == null ? NotFound() : View(parceiro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar confirmação de exclusão ID: {Id}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Confirma a exclusão de um parceiro no banco de dados.
        /// </summary>
        /// <param name="id">O identificador do parceiro.</param>
        /// <returns>Redireciona para o Index após a exclusão.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                await _repository.Excluir(id);
                TempData["Sucesso"] = "Parceiro excluído com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir parceiro ID: {Id}", id);
                TempData["Erro"] = "Não foi possível excluir o parceiro devido a uma dependência ou erro interno.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}