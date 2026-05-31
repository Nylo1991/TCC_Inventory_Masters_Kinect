using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Gerencia notificações do sistema (alertas, eventos e mensagens).
    /// Usado para dashboard e alertas em tempo real.
    /// </summary>
    public class NotificacaoRepository
    {
        private readonly FirestoreDb _db;
        private readonly string _colecao = "Notificacoes";

        public NotificacaoRepository(FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
        }

        /// <summary>
        /// Adiciona nova notificação no Firestore.
        /// </summary>
        public async Task Adicionar(Notificacao notif)
        {
            notif.DataHora = DateTime.UtcNow;

            await _db.Collection(_colecao).AddAsync(notif);
        }

        /// <summary>
        /// Lista notificações mais recentes.
        /// </summary>
        public async Task<List<Notificacao>> ListarTodos()
        {
            var snapshot = await _db.Collection(_colecao).GetSnapshotAsync();

            return snapshot.Documents
                .Select(d =>
                {
                    var n = d.ConvertTo<Notificacao>();
                    n.Id = d.Id;
                    return n;
                })
                .OrderByDescending(x => x.DataHora)
                .ToList();
        }
    }
}