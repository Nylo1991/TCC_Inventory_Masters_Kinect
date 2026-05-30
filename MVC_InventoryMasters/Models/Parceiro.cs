using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.Models
{
    public class Parceiro    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do parceiro é obrigatório.")]
        public string Nome { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; }

        public string Telefone { get; set; }

        public bool Ativo { get; set; } // Flag para saber se o parceiro deve receber alertas
    }
}