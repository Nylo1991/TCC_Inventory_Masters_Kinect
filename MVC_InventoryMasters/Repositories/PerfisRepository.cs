using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Repositório responsável pela leitura
    /// dos perfis cadastrados no Firestore.
    /// </summary>
    public class PerfisRepository
    {
        private readonly FirestoreDb _firestore;

        /// <summary>
        /// Inicializa o repositório de perfis.
        /// </summary>
        /// <param name="firebaseService">
        /// Serviço responsável pela conexão com o Firebase.
        /// </param>
        public PerfisRepository(FirebaseService firebaseService)
        {
            _firestore = firebaseService.Firestore;
        }

        /// <summary>
        /// Retorna todos os perfis cadastrados
        /// na coleção Perfis.
        /// </summary>
        /// <returns>
        /// Lista contendo os perfis disponíveis.
        /// </returns>
        public async Task<List<Perfil>> ListarTodos()
        {
            var snapshot = await _firestore
                .Collection("PerfilUsuario")
                .GetSnapshotAsync();

            return snapshot.Documents
                .Select(doc => doc.ConvertTo<Perfil>())
                .ToList();
        }
    }
}