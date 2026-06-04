using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly PerfisRepository _perfisRepository;

        /// <summary>
        /// Injeta os repositórios necessários para
        /// gerenciamento de usuários e perfis.
        /// </summary>
        /// <param name="repository">
        /// Repositório responsável pelos usuários.
        /// </param>
        /// <param name="perfisRepository">
        /// Repositório responsável pela leitura dos perfis.
        /// </param>
        public UsuariosController(
            UsuariosRepository repository,
            PerfisRepository perfisRepository)
        {
            _repository = repository;
            _perfisRepository = perfisRepository;
        }

        /// <summary>
        /// Carrega os perfis cadastrados no Firebase
        /// para utilização nos DropDownLists.
        /// </summary>
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
        /// Lista todos os usuários cadastrados.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var usuarios = await _repository.ListarTodos();
            return View(usuarios);
        }

        /// <summary>
        /// Exibe a tela de criação de usuário.
        /// Também carrega os perfis disponíveis.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarPerfis();
            return View();
        }

        /// <summary>
        /// Salva um novo usuário no Firestore.
        /// </summary>
        /// <param name="usuario">
        /// Dados do usuário preenchidos no formulário.
        /// </param>
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

        /// <summary>
        /// Exibe os dados do usuário para edição.
        /// </summary>
        /// <param name="id">
        /// Identificador do usuário.
        /// </param>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var usuario = await _repository.BuscarPorId(id);

            if (usuario == null)
                return NotFound();

            await CarregarPerfis();

            return View(usuario);
        }

        /// <summary>
        /// Salva as alterações realizadas em um usuário.
        /// </summary>
        /// <param name="usuario">
        /// Objeto contendo os dados atualizados.
        /// </param>
        /// <summary>
        /// Salva as alterações realizadas em um usuário.
        /// </summary>
        /// <param name="usuario">
        /// Objeto contendo os dados atualizados.
        /// </param>
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

            if (existente == null)
                return NotFound();

            bool houveAlteracao =
                existente.Nome != usuario.Nome ||
                existente.Email != usuario.Email ||
                existente.Perfil != usuario.Perfil ||
                existente.Ativo != usuario.Ativo;

            if (!houveAlteracao)
            {
                ViewBag.Aviso = "Nenhuma alteração foi realizada no usuário.";

                await CarregarPerfis();

                return View(usuario);
            }

            // Mantém a senha atual
            usuario.Senha = existente.Senha;

            // Mantém a data de cadastro
            usuario.Data_Cadastro = existente.Data_Cadastro;

            await _repository.Atualizar(usuario);

            TempData["Sucesso"] = "Usuário atualizado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Exibe a tela de confirmação para exclusão.
        /// </summary>
        /// <param name="id">
        /// Identificador do usuário.
        /// </param>
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
        /// <param name="id">
        /// Identificador do usuário.
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