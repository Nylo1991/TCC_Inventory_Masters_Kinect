using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;
using System.Linq;

namespace MVC_InventoryMasters.Repositories
{
    public class ParametrosSistemaRepository
    {
        private readonly string _colecao = "Parametros";
        private readonly FirestoreDb _db;

        public ParametrosSistemaRepository(FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
        }
     
        public ParametrosSistema Buscar()
        {
            // Busca o documento "configuracoes" dentro da coleção "Parametros"
            var docRef = _db.Collection(_colecao).Document("configuracoes");
            var snapshot = docRef.GetSnapshotAsync().Result;

            if (snapshot.Exists)
            { 
                return snapshot.ConvertTo<ParametrosSistema>();
            }
            // Retorna uma configuração padrão se o documento não existir
            return new ParametrosSistema();
        }
        
        public void Salvar(ParametrosSistema parametros)
        {
            var docRef = _db.Collection(_colecao).Document("configuracoes");
            docRef.SetAsync(parametros).Wait();
        }
    }
}