using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryMaster.Pages;

public class AjudaModel : PageModel
{
    private readonly ILogger<AjudaModel> _logger;

    public AjudaModel(ILogger<AjudaModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}