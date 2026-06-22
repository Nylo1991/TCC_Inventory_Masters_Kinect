using Google.Cloud.Firestore;
using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.Models
{
    [FirestoreData]
    public class Empresa
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
        [StringLength(120, ErrorMessage = "O nome da empresa deve ter no máximo 120 caracteres.")]
        public string? Nome { get; set; }

        [FirestoreProperty]
        [StringLength(18, ErrorMessage = "O CNPJ deve ter no máximo 18 caracteres.")]
        public string? Cnpj { get; set; }

        [FirestoreProperty]
        public DateTime Data_Cadastro { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public bool Ativo { get; set; } = true;
    }
}
