using InventoryMaster.Models;
using InventoryMaster_MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryMaster_MVC.Pages
{
    public class EstoqueModel : PageModel
    {
        private readonly FirebaseService
            _firebaseService;

        public List<MedicaoVolume>
            Entradas
        { get; set; } = new();

        public EstoqueModel(
            FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task OnGetAsync()
        {
            Entradas =
                await _firebaseService
                    .ObterMedicoesAsync();
        }

        public IActionResult OnPostEnviarEmail()
        {
            return new JsonResult(new
            {
                sucesso = true
            });
        }
    }
}