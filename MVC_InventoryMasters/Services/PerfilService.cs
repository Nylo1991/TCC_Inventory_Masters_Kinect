using Google.Cloud.Firestore;

namespace MVC_InventoryMasters.Services
{
    public class PerfilService
    {
        private readonly FirestoreDb _db;

        public PerfilService(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<List<string>> ObterPerfisAsync()
        {
            var snapshot = await _db
                .Collection("Perfis")
                .GetSnapshotAsync();

            return snapshot.Documents
                .Select(x => x.GetValue<string>("Perfil"))
                .ToList();
        }
    }
}
