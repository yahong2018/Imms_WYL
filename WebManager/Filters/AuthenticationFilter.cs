using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Imms.WebManager.Filters
{
    // public class AuthenticationFilter : IAuthorizationFilter
    // {
    //     public void OnAuthorization(AuthorizationFilterContext context)
    //     {
    //         if (context.Filters.Any(x => x is Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter))
    //         {
    //             return;
    //         }
    //         if (!context.HttpContext.User.Identity.IsAuthenticated)
    //         {                
    //             // RedirectResult result = new RedirectResult("~/login");                                
    //             // context.Result = result;

    //             context.Result = new UnauthorizedResult();
    //         }
    //     }
    // }
}
