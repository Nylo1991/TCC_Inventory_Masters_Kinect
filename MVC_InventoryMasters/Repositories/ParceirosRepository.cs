using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Responsável por realizar operações na coleção
    /// "Parceiros" do Firebase Firestore.
    ///
    /// Esta classe centraliza toda a comunicação
    /// entre a aplicação MVC e o banco de dados.
    /// </summary>
    public class ParceirosRepository
    {
        /// <summary>
        /// Nome da coleção no Firestore.
        /// Caso o nome da coleção mude futuramente,
        /// basta alterar apenas esta variável.
        /// </summary>
        private readonly string _colecao = "Parceiros";

        /// <summary>
        /// Instância do Firestore utilizada para
        /// executar consultas e operações no banco.
        /// </summary>
        private readonly FirestoreDb _db;

        /// <summary>
        /// Construtor da classe.
        ///
        /// Recebe o FirebaseService por Injeção de Dependência
        /// e obtém a conexão já configurada com o Firestore.
        /// </summary>
        /// <param name="firebaseService">
        /// Serviço responsável pela conexão com o Firebase.
        /// </param>
        public ParceirosRepository(FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
        }

        /// <summary>
        /// Busca todos os parceiros cadastrados
        /// na coleção "Parceiros" do Firestore.
        /// </summary>
        /// <returns>
        /// Lista contendo todos os parceiros encontrados.
        /// </returns>
        public async Task<List<Parceiro>> ListarTodos()
        {
            List<Parceiro> lista = new();

            QuerySnapshot documentos = await _db
                .Collection(_colecao)
                .GetSnapshotAsync();

            foreach (DocumentSnapshot doc in documentos.Documents)
            {
                Parceiro parceiro = doc.ConvertTo<Parceiro>();

                parceiro.Id = doc.Id;

                lista.Add(parceiro);
            }

            return lista;
        }

        /// <summary>
        /// Busca um parceiro específico pelo ID do documento.
        /// </summary>
        /// <param name="id">
        /// ID do documento no Firestore.
        /// </param>
        /// <returns>
        /// Parceiro encontrado ou null.
        /// </returns>
        public async Task<Parceiro?> BuscarPorId(string id)
        {
            DocumentSnapshot documento = await _db
                .Collection(_colecao)
                .Document(id)
                .GetSnapshotAsync();

            if (!documento.Exists)
                return null;

            Parceiro parceiro = documento.ConvertTo<Parceiro>();

            parceiro.Id = documento.Id;

            return parceiro;
        }

        /// <summary>
        /// Pesquisa parceiros por Nome, Email ou ID.
        /// </summary>
        /// <param name="termo">
        /// Texto digitado pelo usuário.
        /// </param>
        /// <returns>
        /// Lista de parceiros encontrados.
        /// </returns>
        public async Task<List<Parceiro>> Pesquisar(string termo)
        {
            var parceiros = await ListarTodos();

            if (string.IsNullOrWhiteSpace(termo))
                return parceiros;

            termo = termo.ToLower();

            return parceiros
    .Where(p =>
        (p.Id ?? "").ToLower().Contains(termo) ||
        (p.Nome ?? "").ToLower().Contains(termo) ||
        (p.Email ?? "").ToLower().Contains(termo) ||
        (p.Empresa ?? "").ToLower().Contains(termo) ||
        (p.Telefone ?? "").ToLower().Contains(termo))
    .ToList();
        }

        /// <summary>
        /// Adiciona um novo parceiro na coleção Parceiros.
        /// </summary>
        /// <param name="parceiro">
        /// Objeto contendo os dados do parceiro.
        /// </param>
        public async Task Adicionar(Parceiro parceiro)
        {
            var dados = new Dictionary<string, object>
    {
        { "Nome", parceiro.Nome ?? string.Empty },
        { "Email", parceiro.Email ?? string.Empty },
        { "Telefone", parceiro.Telefone ?? string.Empty },
        { "Empresa", parceiro.Empresa ?? string.Empty },
        { "Endereco", parceiro.Endereco ?? string.Empty },
        { "Data_Cadastro", DateTime.UtcNow },
        { "Ativo", parceiro.Ativo }
    };

            await _db
                .Collection(_colecao)
                .AddAsync(dados);
        }

        /// <summary>
        /// Atualiza os dados de um parceiro existente.
        /// </summary>
        /// <param name="parceiro">
        /// Dados atualizados do parceiro.
        /// </param>
        public async Task Atualizar(Parceiro parceiro)
        {
            await _db
                .Collection(_colecao)
                .Document(parceiro.Id)
                .UpdateAsync(new Dictionary<string, object>
                {
            { "Nome", parceiro.Nome ?? string.Empty },
            { "Email", parceiro.Email ?? string.Empty },
            { "Telefone", parceiro.Telefone ?? string.Empty },
            { "Empresa", parceiro.Empresa ?? string.Empty },
            { "Endereco", parceiro.Endereco ?? string.Empty },
            { "Ativo", parceiro.Ativo }
                });
        }

        /// <summary>
        /// Remove um parceiro da coleção Parceiros.
        /// </summary>
        /// <param name="id">
        /// ID do documento que será removido.
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