using Imms.WebManager.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Imms.WebManager.Filters
{
    public class ExtJsResponseBodyFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Request.Method != "POST")
            {
                return;
            }
            if(context.Controller is LoginController){
                return;
            }

            if (context.Result is ObjectResult)
            {
                object value = (context.Result as ObjectResult).Value;                
                context.Result = new ObjectResult((new ExtJsApiCallResult()
                {
                    Success = true,
                    Data = value
                }).ToJson());
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }

    public class ExtJsApiCallResult
    {
        public bool Success { get; set; }
        public object Data { get; set; }
    }
}