using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Models;
using System.Diagnostics;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    [AllowAnonymous]
    public IActionResult Error()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature?.Error != null)
        {
            // Mantém o detalhe técnico fora da tela e preserva o RequestId para rastreamento no log.
            HttpContext.RequestServices
                .GetService<ILogger<HomeController>>()?
                .LogError(exceptionFeature.Error, "Erro não tratado na rota {Path}.", exceptionFeature.Path);
        }

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
