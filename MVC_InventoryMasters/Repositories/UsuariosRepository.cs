using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;
using Microsoft.Extensions.Logging;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Repositório responsável pelo gerenciamento dos usuários cadastrados
    /// no sistema, realizando operações de consulta, cadastro,
    /// atualização e exclusão de registros armazenados no Firebase Firestore.
    /// </summary>
    /// <remarks>
    /// Esta classe centraliza o acesso à coleção de usuários,
    /// abstraindo as operações de persistência e recuperação de dados.
    /// </remarks>
    public class UsuariosRepository : IUsuariosRepository
    {
        /// <summary>
        /// Nome da coleção no Firestore.
        /// </summary>
        private readonly string _colecao = "Usuarios";
        private readonly ILogger<UsuariosRepository> _logger;
        private readonly ContextoUsuarioService _contextoUsuario;

        /// <summary>
        /// Instância do Firestore usada para comunicação com o banco.
        /// </summary>
        private readonly FirestoreDb _db;

        /// <summary>
        /// Construtor do repositório.
        ///
        /// Recebe o FirebaseService via injeção de dependência
        /// e inicializa a conexão com o Firestore.
        /// </summary>
        /// <param name="firebaseService">
        /// Serviço responsável pela configuração do Firebase.
        /// </param>
        /// <param name="logger">
        /// Serviço responsável pelo registro de logs da aplicação.
        /// </param>
        public UsuariosRepository(
            FirebaseService firebaseService,
            ILogger<UsuariosRepository> logger,
            ContextoUsuarioService contextoUsuario)
        {
            _db = firebaseService.Firestore;
            _logger = logger;
            _contextoUsuario = contextoUsuario;
        }
        /// <summary>
        /// Retorna todos os usuários cadastrados
        /// na coleção "Usuarios".
        /// </summary>
        /// <returns>
        /// Lista de usuários.
        /// </returns>
        public async Task<List<Usuario>> ListarTodos()
        {
            try
            {
                List<Usuario> lista = new();

                QuerySnapshot snapshot = await _db
                    .Collection(_colecao)
                    .GetSnapshotAsync();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    Usuario usuario = doc.ConvertTo<Usuario>();
                    usuario.Id = doc.Id;

                    lista.Add(usuario);
                }

                return lista;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao listar usuários.");

                return new List<Usuario>();
            }
        }

        public async Task<List<Usuario>> ListarPorEmpresa(string? empresaId = null)
        {
            string empresa = string.IsNullOrWhiteSpace(empresaId)
                ? _contextoUsuario.ObterEmpresaId()
                : empresaId;

            var usuarios = await ListarTodos();

            // Registros antigos sem EmpresaId continuam visíveis no contexto global até a migração dos dados.
            return usuarios
                .Where(u => u.EmpresaId == empresa ||
                            (empresa == ContextoUsuarioService.EmpresaPadraoId &&
                             string.IsNullOrWhiteSpace(u.EmpresaId)))
                .ToList();
        }

        /// <summary>
        /// Busca um usuário específico pelo ID do documento.
        /// </summary>
        /// <param name="id">
        /// ID do usuário no Firestore.
        /// </param>
        /// <returns>
        /// Usuário encontrado ou null caso não exista.
        /// </returns>
        public async Task<Usuario?> BuscarPorId(string id)
        {
            try
            {
                DocumentSnapshot doc = await _db
                    .Collection(_colecao)
                    .Document(id)
                    .GetSnapshotAsync();

                if (!doc.Exists)
                    return null;

                Usuario usuario = doc.ConvertTo<Usuario>();
                usuario.Id = doc.Id;

                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao buscar usuário {Id}.",
                    id);

                return null;
            }
        }

        public async Task<Usuario?> BuscarPorEmail(string email)
        {
            try
            {
                string emailNormalizado = email.Trim().ToLowerInvariant();

                var snapshot = await _db
                    .Collection(_colecao)
                    .WhereEqualTo("Email", emailNormalizado)
                    .GetSnapshotAsync();

                var doc = snapshot.Documents.FirstOrDefault();

                if (doc == null)
                {
                    var usuarios = await ListarTodos();
                    return usuarios.FirstOrDefault(u =>
                        string.Equals(u.Email?.Trim(), emailNormalizado, StringComparison.OrdinalIgnoreCase));
                }

                var usuario = doc.ConvertTo<Usuario>();
                usuario.Id = doc.Id;
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por e-mail.");
                return null;
            }
        }

        /// <summary>
        /// Adiciona um novo usuário no Firestore.
        /// </summary>
        /// <param name="usuario">
        /// Objeto contendo os dados do usuário.
        /// </param>
        public async Task Adicionar(Usuario usuario)
        {
            try
            {
                usuario.EmpresaId = string.IsNullOrWhiteSpace(usuario.EmpresaId)
                    ? _contextoUsuario.ObterEmpresaId()
                    : usuario.EmpresaId;

                var dados = new Dictionary<string, object>
        {
            { "Nome", usuario.Nome ?? "" },
            { "Email", usuario.Email ?? "" },
            { "Perfil", usuario.Perfil ?? "" },
            { "PerfilId", usuario.PerfilId ?? "" },
            { "EmpresaId", usuario.EmpresaId ?? "" },
            { "Empresa", usuario.Empresa ?? "" },
            { "Senha", usuario.Senha ?? "" },
            { "Data_Cadastro", DateTime.UtcNow },
            { "Ativo", usuario.Ativo }
        };

                await _db
                    .Collection(_colecao)
                    .AddAsync(dados);

                _logger.LogInformation(
                    "Usuário {Email} cadastrado com sucesso.",
                    usuario.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao cadastrar usuário.");

                throw new Exception(
                    "Não foi possível cadastrar o usuário.");
            }
        }

        /// <summary>
        /// Atualiza os dados de um usuário existente.
        /// </summary>
        /// <param name="usuario">
        /// Objeto contendo os dados atualizados.
        /// </param>
        public async Task Atualizar(Usuario usuario)
        {
            try
            {
                await _db
                    .Collection(_colecao)
                    .Document(usuario.Id)
                    .UpdateAsync(new Dictionary<string, object>
                    {
                { "Nome", usuario.Nome ?? "" },
                { "Email", usuario.Email ?? "" },
                { "Perfil", usuario.Perfil ?? "" },
                { "PerfilId", usuario.PerfilId ?? "" },
                { "EmpresaId", usuario.EmpresaId ?? "" },
                { "Empresa", usuario.Empresa ?? "" },
                { "Senha", usuario.Senha ?? "" },
                { "Ativo", usuario.Ativo }
                    });

                _logger.LogInformation(
                    "Usuário {Id} atualizado com sucesso.",
                    usuario.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao atualizar usuário {Id}.",
                    usuario.Id);

                throw new Exception(
                    "Não foi possível atualizar o usuário.");
            }
        }

        /// <summary>
        /// Remove um usuário da coleção.
        /// </summary>
        /// <param name="id">
        /// ID do usuário a ser removido.
        /// </param>
        public async Task Excluir(string id)
        {
            try
            {
                await _db
                    .Collection(_colecao)
                    .Document(id)
                    .DeleteAsync();

                _logger.LogInformation(
                    "Usuário {Id} removido com sucesso.",
                    id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao excluir usuário {Id}.",
                    id);

                throw new Exception(
                    "Não foi possível excluir o usuário.");
            }
        }

        public async Task AtualizarStatus(string id, bool ativo)
        {
            try
            {
                await _db
                    .Collection(_colecao)
                    .Document(id)
                    .UpdateAsync("Ativo", ativo);

                _logger.LogInformation("Status do usuário {Id} atualizado para {Ativo}.", id, ativo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do usuário {Id}.", id);
                throw new Exception("Não foi possível atualizar o status do usuário.");
            }
        }
    }
}
