using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;
using System.Linq;

namespace MVC_InventoryMasters.Repositories
{
    public class ParametrosSistemaRepository
    {
        private readonly string _colecao = "parametrosSistema";
        private readonly FirestoreDb _db;

        public ParametrosSistemaRepository(FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
        }

        public ParametrosSistema Buscar()
        {
            var docRef = _db
                .Collection("parametrosSistema")
                .Document("configuracao");

            var snapshot =
                docRef.GetSnapshotAsync().Result;

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<ParametrosSistema>();
            }

            return new ParametrosSistema();
        }

        public void Salvar(ParametrosSistema parametros)
        {
            parametros.DataAtualizacao =
                DateTime.UtcNow;

            var docRef = _db
                .Collection("parametrosSistema")
                .Document("configuracao");

            docRef.SetAsync(parametros).Wait();
        }
    }
}