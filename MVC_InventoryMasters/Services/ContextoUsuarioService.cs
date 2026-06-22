using Microsoft.AspNetCore.Http;
using MVC_InventoryMasters.Models;
using System.Security.Claims;

namespace MVC_InventoryMasters.Services
{
    public class ContextoUsuarioService
    {
        public const string EmpresaPadraoId = "global";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContextoUsuarioService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string ObterEmpresaId()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Enquanto o login MVC não estiver implementado, o fallback global mantém compatibilidade com dados antigos.
            return httpContext?.User.FindFirst("EmpresaId")?.Value
                ?? httpContext?.Request.Headers["X-Empresa-Id"].FirstOrDefault()
                ?? EmpresaPadraoId;
        }

        public string? ObterUsuarioId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public string? ObterPerfil()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("Perfil")?.Value;
        }

        public bool UsuarioEstaAutenticado()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;
        }
    }
}
