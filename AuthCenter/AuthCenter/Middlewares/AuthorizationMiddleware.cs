using AuthCenter.Data;
using Microsoft.AspNetCore.Identity;

namespace AuthCenter.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public AuthorizationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/API"))
            {
                // 检查是否带有 Token 请求头
                if (context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) && authorizationHeader.Count > 0)
                {
                    var token = authorizationHeader[0].Replace("Bearer ", string.Empty);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // 中间件逻辑...
                    }
                }
                else
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Token is required for API access.");
                }
            }
            else
            {
                // 如果不是 API/Company 路径，直接继续请求管道
                await _next(context);
            }
        }
    }

    public static class AuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorizationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}
