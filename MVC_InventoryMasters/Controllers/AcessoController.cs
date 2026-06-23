using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.ViewModels;
using System.Security.Claims;

namespace MVC_InventoryMasters.Controllers
{
    public class AcessoController : Controller
    {
        private readonly TokenAcessoKinectService _tokenService;
        private readonly EmailTokenService _emailService;
        private readonly UsuariosRepository _usuariosRepository;
        private readonly LogsSistemaRepository _logsRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AcessoController> _logger;

        public AcessoController(
            TokenAcessoKinectService tokenService,
            EmailTokenService emailService,
            UsuariosRepository usuariosRepository,
            LogsSistemaRepository logsRepository,
            IConfiguration configuration,
            ILogger<AcessoController> logger)
        {
            _tokenService = tokenService;
            _emailService = emailService;
            _usuariosRepository = usuariosRepository;
            _logsRepository = logsRepository;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginEmailViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarToken(LoginEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Login", model);

            try
            {
                var resultado = await _tokenService.GerarTokenParaEmail(model.Email!);

                if (!resultado.Sucesso || resultado.Token == null || resultado.Usuario == null)
                {
                    TempData["Erro"] = resultado.Mensagem;
                    return View("Login", model);
                }

                int validadeMinutos = _configuration.GetValue<int?>("KinectAccess:TokenValidityMinutes") ?? 15;

                await _emailService.EnviarTokenKinect(
                    resultado.Usuario.Email!,
                    resultado.Usuario.Nome ?? "usuário",
                    resultado.Token,
                    validadeMinutos);

                await _logsRepository.Registrar(
                    "TokenEnviado",
                    "Token de acesso enviado ao e-mail do usuário.",
                    "Informacao",
                    resultado.Usuario.Email,
                    resultado.Usuario.Id,
                    resultado.Usuario.EmpresaId);

                TempData["Sucesso"] = "Token enviado para o e-mail informado.";
                TempData["EmailToken"] = resultado.Usuario.Email;

                return RedirectToAction(nameof(ValidarToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao solicitar token de acesso.");
                TempData["Erro"] = "Não foi possível enviar o token. Tente novamente.";
                return View("Login", model);
            }
        }

        [HttpGet]
        public IActionResult ValidarToken()
        {
            return View(new ValidarTokenViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarToken(ValidarTokenViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _tokenService.ValidarToken(model.Token);

            if (!resultado.TokenValido || string.IsNullOrWhiteSpace(resultado.Email))
            {
                TempData["Erro"] = resultado.Mensagem ?? "Token inválido.";
                return View(model);
            }

            var usuario = await _usuariosRepository.BuscarPorEmail(resultado.Email);

            if (usuario == null || !usuario.Ativo)
            {
                await _logsRepository.Registrar("LoginRecusado", "Usuário não encontrado ou inativo após validação de token.", "Aviso", resultado.Email);
                TempData["Erro"] = "Usuário não encontrado ou inativo.";
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id ?? string.Empty),
                new(ClaimTypes.Name, usuario.Nome ?? usuario.Email ?? string.Empty),
                new(ClaimTypes.Email, usuario.Email ?? string.Empty),
                new(ClaimTypes.Role, usuario.Perfil ?? string.Empty),
                new("Perfil", usuario.Perfil ?? string.Empty),
                new("EmpresaId", string.IsNullOrWhiteSpace(usuario.EmpresaId)
                    ? ContextoUsuarioService.EmpresaPadraoId
                    : usuario.EmpresaId),
                new("Empresa", usuario.Empresa ?? string.Empty)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            await _logsRepository.Registrar(
                "Login",
                "Login realizado com token válido.",
                "Informacao",
                usuario.Email,
                usuario.Id,
                usuario.EmpresaId);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Negado()
        {
            return View();
        }
    }
}
