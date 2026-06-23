using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.ViewModels
{
    public class LoginEmailViewModel
    {
        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }
    }
}
