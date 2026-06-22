using Microsoft.AspNetCore.Mvc;

namespace MVC_InventoryMasters.Filters
{
    public class PermissaoAuthorizeAttribute : TypeFilterAttribute
    {
        public PermissaoAuthorizeAttribute(string permissao)
            : base(typeof(PermissaoFilter))
        {
            Arguments = new object[] { permissao };
        }
    }
}
