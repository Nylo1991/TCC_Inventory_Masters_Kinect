using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Responsável por realizar todas as operações
    /// na coleção "Usuarios" do Firebase Firestore.
    ///
    /// Este repositório centraliza o CRUD de usuários,
    /// garantindo comunicação entre o MVC e o banco de dados.
    ///
    /// Operações disponíveis:
    /// - Listar usuários
    /// - Buscar por ID
    /// - Criar usuário
    /// - Atualizar usuário
    /// - Excluir usuário
    /// </summary>
    public class UsuariosRepository
    {
        /// <summary>
        /// Nome da coleção no Firestore.
        /// </summary>
        private readonly string _colecao = "Usuarios";

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
        public UsuariosRepository(FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
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

        /// <summary>
        /// Adiciona um novo usuário no Firestore.
        /// </summary>
        /// <param name="usuario">
        /// Objeto contendo os dados do usuário.
        /// </param>
        public async Task Adicionar(Usuario usuario)
        {
            var dados = new Dictionary<string, object>
            {
                { "Nome", usuario.Nome ?? "" },
                { "Email", usuario.Email ?? "" },
                { "Perfil", usuario.Perfil ?? "" },
                { "Senha", usuario.Senha ?? "" },
                { "Data_Cadastro", DateTime.UtcNow },
                { "Ativo", usuario.Ativo }
            };

            await _db
                .Collection(_colecao)
                .AddAsync(dados);
        }

        /// <summary>
        /// Atualiza os dados de um usuário existente.
        /// </summary>
        /// <param name="usuario">
        /// Objeto contendo os dados atualizados.
        /// </param>
        public async Task Atualizar(Usuario usuario)
        {
            await _db
                .Collection(_colecao)
                .Document(usuario.Id)
                .UpdateAsync(new Dictionary<string, object>
                {
                    { "Nome", usuario.Nome ?? "" },
                    { "Email", usuario.Email ?? "" },
                    { "Perfil", usuario.Perfil ?? "" },
                    { "Senha", usuario.Senha ?? "" },
                    { "Ativo", usuario.Ativo }
                });
        }

        /// <summary>
        /// Remove um usuário da coleção.
        /// </summary>
        /// <param name="id">
        /// ID do usuário a ser removido.
        /// </param>
        public async Task Excluir(string id)
        {
            await _db
                .Collection(_colecao)
                .Document(id)
                .DeleteAsync();
        }
    }
}