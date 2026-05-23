using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventoryMaster.Models;
using InventoryMaster.Data;

namespace InventoryMaster.Pages;

public class ParceirosModel : PageModel
{
    private readonly ParceiroRepository _repo = new();

    public List<Parceiro> Parceiros { get; set; } = new();

    [BindProperty]
    public Parceiro NovoParceiro { get; set; }

    public void OnGet()
    {
        Parceiros = _repo.ListarParceiro();
    }

    public IActionResult OnPost()
    {
        if (NovoParceiro != null)
        {
            _repo.Inserir(NovoParceiro);
        }

        return RedirectToPage();
    }
}