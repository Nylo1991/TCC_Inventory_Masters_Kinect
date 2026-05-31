using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;

namespace MVC_InventoryMasters.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento
    /// dos usuários do sistema Inventory Masters.
    ///
    /// Fluxo padrão:
    /// View -> Controller -> Repository -> Firestore
    ///
    /// Funções:
    /// - Listar usuários
    /// - Criar usuário
    /// - Editar usuário
    /// - Excluir usuário
    /// </summary>
    public class UsuariosController : Controller
    {
        private readonly UsuariosRepository _repository;

        /// <summary>
        /// Injeção de dependência do repositório de usuários.
        /// </summary>
        /// <param name="repository">
        /// Classe responsável pela comunicação com o Firestore.
        /// </param>
        public UsuariosController(UsuariosRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Lista todos os usuários cadastrados.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var usuarios = await _repository.ListarTodos();
            return View(usuarios);
        }

        /// <summary>
        /// Exibe a tela de criação de usuário.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Salva um novo usuário no Firestore.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            await _repository.Adicionar(usuario);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Exibe os dados do usuário para edição.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var usuario = await _repository.BuscarPorId(id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        /// <summary>
        /// Salva as alterações feitas no usuário.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Usuario usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            var existente = await _repository.BuscarPorId(usuario.Id);

            if (existente == null)
                return NotFound();

            await _repository.Atualizar(usuario);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Exibe tela de confirmação de exclusão.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var usuario = await _repository.BuscarPorId(id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        /// <summary>
        /// Confirma e executa a exclusão do usuário.
        /// </summary>
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