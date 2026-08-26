using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;
using Microsoft.Extensions.Logging;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Repositório responsável pelo gerenciamento dos parceiros cadastrados,
    /// realizando operações de consulta, pesquisa, cadastro, atualização
    /// e exclusão de registros armazenados no Firebase Firestore.
    /// </summary>
    /// <remarks>
    /// Centraliza o acesso à coleção de parceiros, abstraindo as operações
    /// de persistência e recuperação de dados.
    /// </remarks>
    public class ParceirosRepository : IParceirosRepository
    {
        private readonly string _colecao = "Parceiros";
        private readonly FirestoreDb _db;
        private readonly ILogger<ParceirosRepository> _logger;
        private readonly ContextoUsuarioService _contextoUsuario;

        /// <summary>
        /// Inicializa uma nova instância do repositório de parceiros.
        /// </summary>
        /// <param name="firebaseService">
        /// Serviço responsável por fornecer a conexão com o Firebase Firestore.
        /// </param>
        /// <param name="logger">
        /// Serviço responsável pelo registro de logs da aplicação.
        /// </param>
        public ParceirosRepository(
            FirebaseService firebaseService,
            ILogger<ParceirosRepository> logger,
            ContextoUsuarioService contextoUsuario)
        {
            _db = firebaseService.Firestore;
            _logger = logger;
            _contextoUsuario = contextoUsuario;
        }


        /// <summary>
        /// Recupera todos os parceiros cadastrados no sistema.
        /// </summary>
        /// <returns>
        /// Lista contendo todos os parceiros encontrados.
        /// </returns>
        public async Task<List<Parceiro>> ListarTodos()
        {
            List<Parceiro> lista = new();
            QuerySnapshot documentos = await _db.Collection(_colecao).GetSnapshotAsync();
            foreach (DocumentSnapshot doc in documentos.Documents)
            {
                Parceiro parceiro = doc.ConvertTo<Parceiro>();
                parceiro.Id = doc.Id;
                lista.Add(parceiro);
            }
            return lista;
        }

        public async Task<List<Parceiro>> ListarPorEmpresa(string? empresaId = null)
        {
            string empresa = string.IsNullOrWhiteSpace(empresaId)
                ? _contextoUsuario.ObterEmpresaId()
                : empresaId;

            var parceiros = await ListarTodos();

            return parceiros
                .Where(p => p.EmpresaId == empresa ||
                            (empresa == ContextoUsuarioService.EmpresaPadraoId &&
                             string.IsNullOrWhiteSpace(p.EmpresaId)))
                .ToList();
        }

        /// <summary>
        /// Busca um parceiro utilizando seu identificador único.
        /// </summary>
        /// <param name="id">
        /// Identificador do parceiro.
        /// </param>
        /// <returns>
        /// Objeto do parceiro encontrado ou nulo caso não exista.
        /// </returns>
        public async Task<Parceiro?> BuscarPorId(string id)
        {
            DocumentSnapshot documento = await _db.Collection(_colecao).Document(id).GetSnapshotAsync();
            if (!documento.Exists) return null;
            Parceiro parceiro = documento.ConvertTo<Parceiro>();
            parceiro.Id = documento.Id;
            return parceiro;
        }

        public async Task<List<Parceiro>> Pesquisar(string termo)
        {
            var parceiros = await ListarPorEmpresa();
            if (string.IsNullOrWhiteSpace(termo)) return parceiros;
            termo = termo.ToLower();
            return parceiros.Where(p =>
                (p.Id ?? "").ToLower().Contains(termo) ||
                (p.Nome ?? "").ToLower().Contains(termo) ||
                (p.Email ?? "").ToLower().Contains(termo) ||
                (p.Empresa ?? "").ToLower().Contains(termo) ||
                (p.Telefone ?? "").ToLower().Contains(termo))
            .ToList();
        }

        /// <summary>
        /// Realiza uma pesquisa avançada de parceiros utilizando
        /// filtros de texto, período de cadastro e status.
        /// </summary>
        /// <param name="termo">
        /// Texto utilizado para pesquisa geral.
        /// </param>
        /// <param name="dataInicio">
        /// Data inicial do período de cadastro.
        /// </param>
        /// <param name="dataFim">
        /// Data final do período de cadastro.
        /// </param>
        /// <param name="ativo">
        /// Status do parceiro.
        /// </param>
        /// <returns>
        /// Lista contendo os parceiros que atendem aos filtros informados.
        /// </returns>
        public async Task<List<Parceiro>> FiltrarAvancado(string termo, DateTime? dataInicio, DateTime? dataFim, bool? ativo)
        {
            var lista = await ListarPorEmpresa();

            // 1. Filtro Geral
            if (!string.IsNullOrWhiteSpace(termo))
            {
                var t = termo.ToLower();
                lista = lista.Where(p =>
                    (p.Id ?? "").ToLower().Contains(t) ||
                    (p.Nome ?? "").ToLower().Contains(t) ||
                    (p.Email ?? "").ToLower().Contains(t) ||
                    (p.Empresa ?? "").ToLower().Contains(t) ||
                    (p.Telefone ?? "").ToLower().Contains(t)
                ).ToList();
            }

            // 2. Filtro de Data Início
            if (dataInicio.HasValue)
                lista = lista.Where(p => p.Data_Cadastro.Date >= dataInicio.Value.Date).ToList();

            // 3. Filtro de Data Fim
            if (dataFim.HasValue)
                lista = lista.Where(p => p.Data_Cadastro.Date <= dataFim.Value.Date).ToList();

            // 4. Filtro de Status
            if (ativo.HasValue)
                lista = lista.Where(p => p.Ativo == ativo.Value).ToList();

            return lista;
        }

        /// <summary>
        /// Adiciona um novo parceiro na base de dados.
        /// </summary>
        /// <param name="parceiro">
        /// Objeto contendo os dados do parceiro.
        /// </param>
        /// <returns>
        /// Tarefa assíncrona responsável pela persistência do parceiro.
        /// </returns>
        public async Task Adicionar(Parceiro parceiro)
        {
            parceiro.EmpresaId = string.IsNullOrWhiteSpace(parceiro.EmpresaId)
                ? _contextoUsuario.ObterEmpresaId()
                : parceiro.EmpresaId;

            var dados = new Dictionary<string, object>
            {
                { "Nome", parceiro.Nome ?? string.Empty },
                { "Email", parceiro.Email ?? string.Empty },
                { "Telefone", parceiro.Telefone ?? string.Empty },
                { "Empresa", parceiro.Empresa ?? string.Empty },
                { "EmpresaId", parceiro.EmpresaId ?? string.Empty },
                { "Endereco", parceiro.Endereco ?? string.Empty },
                { "Data_Cadastro", DateTime.UtcNow },
                { "Ativo", parceiro.Ativo }
            };
            await _db.Collection(_colecao).AddAsync(dados);
        }

        /// <summary>
        /// Atualiza os dados de um parceiro existente.
        /// </summary>
        /// <param name="parceiro">
        /// Objeto contendo os dados atualizados do parceiro.
        /// </param>
        /// <returns>
        /// Tarefa assíncrona responsável pela atualização do registro.
        /// </returns>
        public async Task Atualizar(Parceiro parceiro)
        {
            await _db.Collection(_colecao).Document(parceiro.Id).UpdateAsync(new Dictionary<string, object>
            {
                { "Nome", parceiro.Nome ?? string.Empty },
                { "Email", parceiro.Email ?? string.Empty },
                { "Telefone", parceiro.Telefone ?? string.Empty },
                { "Empresa", parceiro.Empresa ?? string.Empty },
                { "EmpresaId", parceiro.EmpresaId ?? string.Empty },
                { "Endereco", parceiro.Endereco ?? string.Empty },
                { "Ativo", parceiro.Ativo }
            });
        }

        /// <summary>
        /// Remove um parceiro da base de dados.
        /// </summary>
        /// <param name="id">
        /// Identificador do parceiro a ser removido.
        /// </param>
        /// <returns>
        /// Tarefa assíncrona responsável pela exclusão do registro.
        /// </returns>
        public async Task Excluir(string id)
        {
            try
            {
                await _db.Collection(_colecao)
                         .Document(id)
                         .DeleteAsync();

                _logger.LogInformation(
                    "Parceiro {Id} removido com sucesso.",
                    id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao excluir parceiro {Id}.",
                    id);

                throw new Exception(
                    "Não foi possível excluir o parceiro.");
            }
        }

        public async Task AtualizarStatus(string id, bool ativo)
        {
            try
            {
                await _db.Collection(_colecao)
                         .Document(id)
                         .UpdateAsync("Ativo", ativo);

                _logger.LogInformation("Status do parceiro {Id} atualizado para {Ativo}.", id, ativo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do parceiro {Id}.", id);
                throw new Exception("Não foi possível atualizar o status do parceiro.");
            }
        }
    }
}
