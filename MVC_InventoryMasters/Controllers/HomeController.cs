using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Repositories;

public class HomeController : Controller
{
    private readonly ParceirosRepository _repository;

    public HomeController(
        ParceirosRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        var parceiros =
            await _repository.ListarTodos();

        return View(parceiros);
    }
}