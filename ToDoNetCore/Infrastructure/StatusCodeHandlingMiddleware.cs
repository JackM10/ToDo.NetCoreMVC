using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ToDoNetCore.Infrastructure
{
    public class StatusCodeHandlingMiddleware
    {
        private RequestDelegate nextDelegate;

        public StatusCodeHandlingMiddleware(RequestDelegate next) => nextDelegate = next;

        public async Task Invoke(HttpContext httpContext)
        {
            await nextDelegate.Invoke(httpContext);

            switch (httpContext.Response.StatusCode)
            {
                case 404:
                    httpContext.Response.Redirect("/StaticPages/IE_PageNotFound/dnserror.html");
                    break;
                case 403:
                    await httpContext.Response.WriteAsync("403 happenes in middlweare");
                    break;
            }
        }

    }
}
