using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Imms.WebManager
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var statusCode = context.Response.StatusCode;
                if (ex is ArgumentException)
                {
                    statusCode = 200;
                }
                try
                {
                    await HandleExceptionAsync(context, ex, statusCode);
                }
                catch
                {
                }
            }
            finally
            {
                var statusCode = context.Response.StatusCode;
                if (statusCode != 101)
                {
                    await ProcessError(context, statusCode);
                }
            }
        }

        private static async Task ProcessError(Microsoft.AspNetCore.Http.HttpContext context, int statusCode)
        {
            var msg = "";
            if (statusCode == 401)
            {
                if (context.Request.Path.ToString() == "/"
                    || context.Request.Path.ToString() == "/home"
                    || context.Request.Path.ToString() == context.Request.PathBase.ToString()
                    || context.Request.Path.ToString() == ""
                )
                {
                    context.Response.Redirect(context.Request.PathBase + "/login");
                }
                else
                {
                    msg = "未授权";
                }
            }
            else if (statusCode == 404)
            {
                msg = "未找到服务";
            }
            else if (statusCode == 502)
            {
                msg = "请求错误";
            }
            else if (statusCode != 200)
            {
                msg = "未知错误";
            }
            if (!string.IsNullOrWhiteSpace(msg))
            {
                try
                {
                    await HandleExceptionAsync(context, statusCode, msg);
                }
                catch
                {
                }
            }
        }

        private static Task HandleExceptionAsync(Microsoft.AspNetCore.Http.HttpContext context, int statusCode, string msg)
        {
            var result = JsonConvert.SerializeObject(new ApiResult() { Success = false, Data = new ApiResultData() { ExceptionCode = statusCode, RequestUrl = context.Request.Path, Message = msg } });
            GlobalConstants.DefaultLogger.Error(result);

            context.Response.ContentType = "application/json;charset=utf-8";
            return context.Response.WriteAsync(result);
        }

        //异常错误信息捕获，将错误信息用Json方式返回
        private static Task HandleExceptionAsync(Microsoft.AspNetCore.Http.HttpContext context, Exception ex, int statusCode)
        {
            StringBuilder builder = new StringBuilder(ex.Message);
            ErrorHandlingMiddleware.GetExceptionMessage(ex, builder);
            ApiResult apiResult = new ApiResult() { Success = false, Data = new ApiResultData() { Message = builder.ToString(), RequestUrl = context.Request.Path, ExceptionCode = statusCode } };
            if (ex is BusinessException)
            {
                apiResult.Data.ExceptionCode = (ex as BusinessException).ExceptionCode;
            }

            var result = JsonConvert.SerializeObject(apiResult);
            context.Response.ContentType = "application/json;charset=utf-8";

            GlobalConstants.DefaultLogger.Error(result);
            GlobalConstants.DefaultLogger.Error(ex.StackTrace);

            return context.Response.WriteAsync(result);
        }

        private static void GetExceptionMessage(Exception ex, StringBuilder builder)
        {
            if (ex.InnerException != null)
            {
                builder.Append("\r\n");
                builder.Append(ex.InnerException.Message);
                Exception innerException = ex.InnerException;
                ErrorHandlingMiddleware.GetExceptionMessage(innerException, builder);
            }
        }
    }
    //扩展方法
    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }

    public class ApiResult
    {
        public bool Success { get; set; } = true;
        public ApiResultData Data { get; set; }
    }

    public class ApiResultData
    {
        public int ExceptionCode { get; set; } = 0;
        public string RequestUrl { get; set; }
        public string Message { get; set; }
    }
}