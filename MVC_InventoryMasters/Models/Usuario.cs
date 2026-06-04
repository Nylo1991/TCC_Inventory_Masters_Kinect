using Google.Cloud.Firestore;
using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.Models
{
    [FirestoreData]
    public class Usuario
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string? Nome { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        /// Validação do formato de e-mail
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$",
           ErrorMessage = "E-mail inválido. O formato correto é nome@dominio.com")]
        public string? Email { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "O perfil é obrigatório.")]
        public string? Perfil { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 20 caracteres.")]
        public string? Senha { get; set; }

        [FirestoreProperty]
        public DateTime? Data_Cadastro { get; set; } = DateTime.Now;

        [FirestoreProperty]
        public bool Ativo { get; set; } = true;
    }
}