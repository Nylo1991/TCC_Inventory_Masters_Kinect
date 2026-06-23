<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
=======
﻿using Microsoft.AspNetCore.Mvc;
>>>>>>> 69278f70785abed625eb15930bd6564a7fd280ec

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
