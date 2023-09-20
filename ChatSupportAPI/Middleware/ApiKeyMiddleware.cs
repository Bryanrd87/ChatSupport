using System.Net;
using System.Security.Claims;

namespace ChatSupport.WebAPI.Middleware;
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyMiddleware> _logger;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;   
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue("x-api-key", out var extractedApiKey))
        {
            _logger.LogWarning("API Key was not provided. Access Denied.");

            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            httpContext.Response.ContentType = "text/plain";

            await httpContext.Response.WriteAsync("API Key was not provided. Access Denied.");

            return;
        }

        var apiKeys = _configuration.GetSection("ApiKeys").Get<Dictionary<string, string>>();

        if (apiKeys.All(k => k.Value != extractedApiKey))
        {
            _logger.LogWarning("Unauthorized: Access denied");

            httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            httpContext.Response.ContentType = "text/plain";

            await httpContext.Response.WriteAsync("Unauthorized: Access denied");

            return;
        }

        if (apiKeys.Any(k => k.Value == extractedApiKey))
        {
            var apiKeyConfig = apiKeys.First(k => k.Value == extractedApiKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, apiKeyConfig.Key)                
            };

            var identity = new ClaimsIdentity(claims, "ApiKey");
            var principal = new ClaimsPrincipal(identity);

            httpContext.User = principal;
        }

        await _next(httpContext);
    }
}
