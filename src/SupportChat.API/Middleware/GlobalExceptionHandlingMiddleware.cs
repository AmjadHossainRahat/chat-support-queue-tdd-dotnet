using Microsoft.AspNetCore.Mvc;
using SupportChat.API.Constants;

namespace SupportChat.API.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("was not found"))
        {
            _logger.LogWarning(
                ex,
                "Request failed because requested resource was not found for {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);

            await WriteProblemDetailsAsync(
                httpContext,
                StatusCodes.Status404NotFound,
                "Resource not found",
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception while processing {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);

            await WriteProblemDetailsAsync(
                httpContext,
                StatusCodes.Status500InternalServerError,
                "Unexpected server error",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail)
    {
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (httpContext.Items.TryGetValue(CorrelationIdMiddleware.HttpContextItemKey, out var correlationId)
            && correlationId is string correlationIdValue)
        {
            problemDetails.Extensions["correlationId"] = correlationIdValue;
            httpContext.Response.Headers[HttpHeaderNames.CorrelationId] = correlationIdValue;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }
}