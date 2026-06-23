using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.ViewModels
{
    public class SolicitarTokenKinectRequest
    {
        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string? Email { get; set; }
    }
}
