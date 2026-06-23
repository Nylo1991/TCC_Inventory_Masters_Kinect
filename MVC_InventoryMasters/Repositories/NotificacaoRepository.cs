using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Repositório responsável pelo gerenciamento das notificações do sistema,
    /// incluindo cadastro, consulta, atualização de status e validação
    /// de notificações pendentes armazenadas no Firebase Firestore.
    /// </summary>
    /// <remarks>
    /// Esta classe centraliza o acesso à coleção de notificações,
    /// abstraindo as operações de persistência e consulta de dados.
    /// </remarks>
    public class NotificacaoRepository
    {
        private readonly FirestoreDb _db;
        private readonly ILogger<NotificacaoRepository> _logger;
        private readonly ContextoUsuarioService _contextoUsuario;
        private readonly string _colecao = "Notificacoes";

        public NotificacaoRepository(
            FirebaseService firebaseService,
            ILogger<NotificacaoRepository> logger,
            ContextoUsuarioService contextoUsuario)
        {
            _db = firebaseService.Firestore;
            _logger = logger;
            _contextoUsuario = contextoUsuario;
        }

        /// <summary>
        /// Adiciona uma nova notificação na base de dados.
        /// </summary>
        /// <param name="notif">
        /// Objeto contendo os dados da notificação a ser armazenada.
        /// </param>
        /// <returns>
        /// Tarefa assíncrona responsável pela persistência da notificação.
        /// </returns>
        public async Task Adicionar(Notificacao notif)
        {
            try
            {
                notif.DataHora = DateTime.UtcNow;
                notif.EmpresaId = string.IsNullOrWhiteSpace(notif.EmpresaId)
                    ? _contextoUsuario.ObterEmpresaId()
                    : notif.EmpresaId;

                await _db.Collection(_colecao).AddAsync(notif);
                _logger.LogInformation("Notificação de {Mensagem} adicionada com sucesso ao Firestore.", notif.Mensagem);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Falha crítica ao tentar adicionar notificação no Firestore.");

                throw new Exception(
                    "Ocorreu um erro ao registrar a notificação.");
            }
        }

        /// <summary>
        /// Recupera todas as notificações cadastradas no sistema.
        /// </summary>
        /// <remarks>
        /// As notificações são retornadas ordenadas pela data de criação,
        /// da mais recente para a mais antiga.
        /// </remarks>
        /// <returns>
        /// Lista contendo todas as notificações encontradas.
        /// </returns>
        public async Task<List<Notificacao>> ListarTodos()
        {
            try
            {
                var snapshot = await _db.Collection(_colecao)
                                        .OrderByDescending("DataHora")
                                        .GetSnapshotAsync();

                return snapshot.Documents.Select(d =>
                {
                    var n = d.ConvertTo<Notificacao>();
                    n.Id = d.Id;
                    return n;
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao recuperar a lista de notificações.");
                return new List<Notificacao>();
            }
        }

        public async Task<List<Notificacao>> ListarPorEmpresa(string? empresaId = null)
        {
            string empresa = string.IsNullOrWhiteSpace(empresaId)
                ? _contextoUsuario.ObterEmpresaId()
                : empresaId;

            var notificacoes = await ListarTodos();

            return notificacoes
                .Where(n => n.EmpresaId == empresa ||
                            (empresa == ContextoUsuarioService.EmpresaPadraoId &&
                             string.IsNullOrWhiteSpace(n.EmpresaId)))
                .OrderByDescending(n => n.DataHora)
                .ToList();
        }

        /// <summary>
        /// Atualiza o status de uma notificação existente.
        /// </summary>
        /// <param name="id">
        /// Identificador único da notificação.
        /// </param>
        /// <param name="novoStatus">
        /// Novo status que será atribuído à notificação.
        /// </param>
        /// <returns>
        /// True quando a atualização for realizada com sucesso.
        /// False caso ocorra alguma falha durante o processo.
        /// </returns>

        public async Task<bool> AtualizarStatus(string id, string novoStatus)
        {
            try
            {
                var docRef = _db.Collection(_colecao).Document(id);
                await docRef.UpdateAsync("StatusEnvio", novoStatus);

                _logger.LogInformation("Sucesso: Status da notificação {Id} foi alterado para {Status}.", id, novoStatus);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar atualizar o status da notificação {Id} para {Status}.", id, novoStatus);
                return false;
            }
        }

        /// <summary>
        /// Verifica se existe alguma notificação
        /// pendente de atendimento.
        ///
        /// Utilizado para evitar a geração
        /// de notificações duplicadas quando
        /// o volume continua acima do limite.
        /// </summary>
        /// <returns>
        /// True se existir uma notificação pendente.
        /// False caso contrário.
        /// </returns>
        public async Task<bool> ExisteNotificacaoPendente()
        {
            try
            {
                QuerySnapshot snapshot = await _db
                    .Collection(_colecao)
                    .WhereEqualTo(
                        "StatusEnvio",
                        "Pendente")
                    .GetSnapshotAsync();

                string empresa = _contextoUsuario.ObterEmpresaId();

                return snapshot.Documents.Any(doc =>
                {
                    var notificacao = doc.ConvertTo<Notificacao>();

                    return notificacao.EmpresaId == empresa ||
                           (empresa == ContextoUsuarioService.EmpresaPadraoId &&
                            string.IsNullOrWhiteSpace(notificacao.EmpresaId));
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao verificar notificações pendentes.");

                return false;
            }
        }
    }
}
