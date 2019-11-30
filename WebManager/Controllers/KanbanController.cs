using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Imms.WebManager.Controllers
{
    [Route("kanban")]
    [TypeFilter(typeof(Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter))]
    public class KanbanController : Controller{
        [Route("line")]
        public IActionResult Index()
        {
            return View("Line");
        } 

        [Route("line_wrapper")]
        public IActionResult LineWrapper(){
            return View("Line_Wrapper");
        }       
    }    
}
