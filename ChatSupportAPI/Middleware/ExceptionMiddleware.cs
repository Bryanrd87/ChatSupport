using ChatSupport.Application.Models;
using ChatSupport.Application.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace ChatSupport.WebAPI.Middleware;
public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex, CancellationToken cancellationToken = default)
    {
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        CustomProblemDetails problem = new CustomProblemDetails();

        switch (ex)
        {
           
            case InvalidUserException InvalidUserException:
                statusCode = HttpStatusCode.NotFound;
                problem = new CustomProblemDetails
                {
                    Title = InvalidUserException.Message,
                    Status = (int)statusCode,
                    Type = nameof(InvalidUserException),
                    Detail = InvalidUserException.InnerException?.Message,
                };
                break;           
            default:
                statusCode = HttpStatusCode.InternalServerError;
                problem = new CustomProblemDetails
                {
                    Title = ex.Message,
                    Status = (int)statusCode,
                    Type = nameof(HttpStatusCode.InternalServerError),
                    Detail = ex.StackTrace,
                };
                break;
        }

        httpContext.Response.StatusCode = (int)statusCode;
        var logMessage = JsonConvert.SerializeObject(problem);
        _logger.LogError(logMessage);
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(logMessage, cancellationToken);
    }
}
