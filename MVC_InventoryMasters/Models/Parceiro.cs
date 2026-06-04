using Google.Cloud.Firestore;
using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.Models
{
    [FirestoreData]
    public class Parceiro
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome do Parceiro")]
        public string? Nome { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "E-mail inválido. Utilize o formato nome@dominio.com")]
        [Display(Name = "E-mail de Contato")]
        public string? Email { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Display(Name = "Telefone")]
        [StringLength(15, ErrorMessage = "Telefone muito longo.")]
        public string? Telefone { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "A empresa é obrigatória.")]
        [StringLength(100, ErrorMessage = "Nome da empresa muito longo.")]
        public string? Empresa { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "O endereço é obrigatório.")]
        [Display(Name = "Endereço")]
        public string? Endereco { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "A data de cadastro é obrigatória.")]
        public DateTime Data_Cadastro { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]        
        [Range(typeof(bool), "true", "true", ErrorMessage = "Você deve marcar o status como Ativo.")]
        public bool Ativo { get; set; } = true;
    }
}