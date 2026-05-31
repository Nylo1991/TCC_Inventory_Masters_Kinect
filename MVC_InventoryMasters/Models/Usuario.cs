using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.Models
{
    [FirestoreData]
    public class Usuario
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "O nome do usuário é obrigatório.")]
        public string? Nome { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string? Email { get; set; }

        [FirestoreProperty]
        public string? Perfil { get; set; }

        [FirestoreProperty]
        public string? Senha { get; set; }

        [FirestoreProperty]
        public DateTime? Data_Cadastro { get; set; }

        [FirestoreProperty]
        public bool Ativo { get; set; }
    }
}