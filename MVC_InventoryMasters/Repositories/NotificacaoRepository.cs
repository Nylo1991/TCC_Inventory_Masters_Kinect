using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;
using System.Collections.Generic;

namespace MVC_InventoryMasters.Repositories
{
    public class NotificacaoRepository
    {
        private readonly string _colecao = "Notificacoes";
        private readonly FirestoreDb _db;

        public NotificacaoRepository(FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
        }

        public List<Notificacao> ListarHistorico()
        {
            List<Notificacao> lista = new List<Notificacao>();

            // Busca todas as notificações salvas
            var documentos = _db.Collection(_colecao).GetSnapshotAsync().Result;

            foreach (var doc in documentos.Documents)
            {
                Notificacao n = doc.ConvertTo<Notificacao>();
                lista.Add(n);
            }

            return lista;
        }

        public void Salvar(Notificacao notificacao)
        {
            // Salva uma nova notificação disparada pelo sistema
            _db.Collection(_colecao).AddAsync(notificacao).Wait();
        }
    }
}