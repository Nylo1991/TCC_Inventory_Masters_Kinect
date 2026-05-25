using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventoryMaster.Models;
using InventoryMaster.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryMaster.Pages;

public class ParceirosModel : PageModel
{
    private readonly ParceiroRepository _repo;


    public ParceirosModel(ParceiroRepository repo)
    {
        _repo = repo;
    }

    public List<Parceiro> Parceiros { get; set; } = new();

    [BindProperty]
    public Parceiro NovoParceiro { get; set; } = new(); 

    public async Task OnGetAsync()
    {
        Parceiros = await _repo.ListarParceiroAsync();
    }

    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrWhiteSpace(NovoParceiro.Nome))
        {
            await _repo.InserirAsync(NovoParceiro);
        }

        return RedirectToPage();
    }
}