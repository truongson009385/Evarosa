using Microsoft.AspNetCore.Http.Extensions;

namespace Evarosa.Utils
{
    public class DomainRedirect
    {
        private readonly RequestDelegate _next;

        public DomainRedirect(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.GetDisplayUrl().Contains("evarosa.vn")) //to check if request URL is your button link
            {
                httpContext.Response.Redirect("https://cosmart.vn", true);

                //string requestDomian = httpContext.Request.Headers["Referer"];   //get "https://qa1.cmsSite.com"
                //if (requestDomian != null)
                //{
                //    requestDomian = requestDomian.Split('/')[2];      //get "qa1.cmsSite.com"
                //    var initialWords = requestDomian.Split(".")[0];    //get "qa1"

                //    httpContext.Response.Redirect("https://" + initialWords + ".exampleApp.com");
                //}
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.  
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DomainRedirect>();
        }
    }
}
