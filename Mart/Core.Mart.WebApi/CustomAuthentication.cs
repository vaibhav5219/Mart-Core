using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Core.Mart.WebApi
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CustomAuthentication
    {
        private readonly RequestDelegate _next;

        public CustomAuthentication(RequestDelegate next)
        {
            _next = next;
        }

        // It can automatic call , before any api call
        public Task Invoke(HttpContext httpContext)
        {
            // Preprocessing Logic write here
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustomAuthenticationExtensions
    {
        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomAuthentication>();
        }
    }
}
