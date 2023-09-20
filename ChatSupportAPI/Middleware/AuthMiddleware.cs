using System.Net;

namespace ChatSupport.WebAPI.Middleware;
public class AuthMiddleware : IMiddleware
{
    private readonly ILogger<AuthMiddleware> _logger;
    public AuthMiddleware(ILogger<AuthMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        await next(httpContext);

        if (httpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("API Key was not provided. Access Denied.");

            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            httpContext.Response.ContentType = "text/plain";

            await httpContext.Response.WriteAsync("API Key was not provided. Access Denied.");
        }

        if (httpContext.Response.StatusCode == (int)HttpStatusCode.Forbidden)
        {
            _logger.LogWarning("Unauthorized: Access denied");

            httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            httpContext.Response.ContentType = "text/plain";

            await httpContext.Response.WriteAsync("Unauthorized: Access denied");
        }
    }
}
