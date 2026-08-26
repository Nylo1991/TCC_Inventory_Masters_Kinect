using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.ViewModels
{
    public class ValidarTokenViewModel
    {
        [Required(ErrorMessage = "Informe o token.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "O token deve ter 6 caracteres.")]
        public string? Token { get; set; }
    }
}
