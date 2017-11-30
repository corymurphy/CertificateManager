using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManager.Logic.MvcMiddleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode code = ParseException(exception);

            var result = ResponseBody(exception.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

        public static HttpStatusCode ParseException(Exception exception)
        {
            switch (exception)
            {
                case UnauthorizedAccessException unauthorizedExceptionType:
                    return HttpStatusCode.Forbidden;
                default:
                    return HttpStatusCode.InternalServerError;

            }

        }

        private static string ResponseBody(string msg)
        {
            var body = new { status = "error", message = msg };
            return JsonConvert.SerializeObject(body);
        }
    }
}
