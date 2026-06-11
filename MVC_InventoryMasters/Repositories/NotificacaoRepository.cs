using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
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
    }
}
