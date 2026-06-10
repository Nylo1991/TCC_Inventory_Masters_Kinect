using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Gerencia as operações de persistência e leitura das notificações no Firestore.
    /// Esta classe atua como uma camada de abstração entre o Controller e o banco de dados.
    /// </summary>
    public class NotificacaoRepository
    {
        private readonly FirestoreDb _db;
        private readonly ILogger<NotificacaoRepository> _logger;
        private readonly string _colecao = "Notificacoes";

        public NotificacaoRepository(FirebaseService firebaseService, ILogger<NotificacaoRepository> logger)
        {
            _db = firebaseService.Firestore;
            _logger = logger;
        }

        /// <summary>
        /// Adiciona uma nova notificação ao Firestore.
        /// </summary>
        /// <param name="notif">Objeto de notificação a ser persistido.</param>
        public async Task Adicionar(Notificacao notif)
        {
            try
            {
                notif.DataHora = DateTime.UtcNow;
                await _db.Collection(_colecao).AddAsync(notif);
                _logger.LogInformation("Notificação de {Mensagem} adicionada com sucesso ao Firestore.", notif.Mensagem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha crítica ao tentar adicionar notificação no Firestore.");
                throw;
            }
        }

        /// <summary>
        /// Recupera todas as notificações, ordenadas da mais recente para a mais antiga.
        /// </summary>
        /// <returns>Uma lista de objetos Notificacao.</returns>
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

        /// <summary>
        /// Atualiza o campo StatusEnvio de uma notificação específica.
        /// </summary>
        /// <param name="id">ID do documento no Firestore.</param>
        /// <param name="novoStatus">Novo status (ex: "Aceito", "Erro").</param>
        /// <returns>True se a operação foi bem-sucedida, false caso contrário.</returns>
        public async Task<bool> AtualizarStatus(string id, string novoStatus)
        {
            try
            {
                var docRef = _db.Collection(_colecao).Document(id);

                // O método UpdateAsync modifica apenas o campo especificado no Firestore
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
    }
}