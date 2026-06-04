using Google.Cloud.Firestore;

namespace MVC_InventoryMasters.Models
{
    /// <summary>
    /// Representa um perfil disponível
    /// para utilização no sistema.
    /// </summary>
    [FirestoreData]
    public class Perfil
    {
        /// <summary>
        /// Identificador do documento.
        /// </summary>
        [FirestoreDocumentId]
        public string? Id { get; set; }

        /// <summary>
        /// Nome do perfil.
        /// </summary>
        [FirestoreProperty("Perfil")]
        public string? Nome { get; set; }
    }
}