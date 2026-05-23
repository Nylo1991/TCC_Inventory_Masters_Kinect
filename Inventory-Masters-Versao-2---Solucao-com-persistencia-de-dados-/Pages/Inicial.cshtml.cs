using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryMaster.Pages;

public class InicialModel : PageModel
{
    private readonly ILogger<InicialModel> _logger;

    public InicialModel(ILogger<InicialModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}
