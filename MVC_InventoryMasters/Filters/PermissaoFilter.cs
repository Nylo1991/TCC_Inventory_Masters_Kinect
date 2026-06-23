using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVC_InventoryMasters.Services;
using System.Security.Claims;

namespace MVC_InventoryMasters.Filters
{
    public class PermissaoFilter : IAuthorizationFilter
    {
        private readonly string _permissao;
        private readonly PermissaoService _permissaoService;

        public PermissaoFilter(string permissao, PermissaoService permissaoService)
        {
            _permissao = permissao;
            _permissaoService = permissaoService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                context.Result = new ChallengeResult();
                return;
            }

            var perfil = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value
                ?? context.HttpContext.User.FindFirst("Perfil")?.Value;

            if (!_permissaoService.PerfilPossuiPermissao(perfil, _permissao))
            {
                context.Result = new RedirectToActionResult("Negado", "Acesso", null);
            }
        }
    }
}
