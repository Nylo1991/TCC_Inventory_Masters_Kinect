using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.Models
{
    [FirestoreData]
    public class Parceiro
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "O nome do parceiro é obrigatório.")]
        public string Nome { get; set; }

        [FirestoreProperty]
        [Required]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; }

        [FirestoreProperty]
        public string Telefone { get; set; }

        [FirestoreProperty]
        public bool Ativo { get; set; }
    }
}