using Microsoft.AspNetCore.Mvc;
/// <summary>
/// Controlador responsável por gerenciar as ações relacionadas à página inicial do sistema
/// </summary>
/// remarks> A página inicial redireciona os usuários para o Dashboard, onde podem visualizar indicadores de desempenho, 
/// alertas e informações relevantes para o sistema.</remarks>
/// param name="HomeController">Controlador da página inicial</param>
/// returns>Redirecionamento para a ação Index do Dashboard</returns>
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Dashboard");
    }
}