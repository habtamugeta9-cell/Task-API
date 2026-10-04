using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TaskApi.Errors;


public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger): IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unhandled exception caught with trace ID {TraceId}",
            httpContext.TraceIdentifier);

        var statusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetail = new ProblemDetails()
        {
            Status = statusCode,
            Title = statusCode == StatusCodes.Status400BadRequest
            ? "Bad Request"
            : "Internal Server Error",
            Detail = exception.Message
        };
        
        httpContext.Response.StatusCode = statusCode;
        
        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetail
        });
    }
}