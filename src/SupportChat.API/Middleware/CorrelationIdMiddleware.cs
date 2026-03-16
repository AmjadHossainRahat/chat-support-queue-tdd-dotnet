using SupportChat.API.Constants;

namespace SupportChat.API.Middleware;

public class CorrelationIdMiddleware
{
    public const string HttpContextItemKey = "CorrelationId";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var correlationId = ResolveCorrelationId(httpContext);

        httpContext.Items[HttpContextItemKey] = correlationId;
        httpContext.Response.Headers[HttpHeaderNames.CorrelationId] = correlationId;

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            _logger.LogInformation(
                "Handling HTTP {Method} {Path} with correlation id {CorrelationId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                correlationId);

            await _next(httpContext);

            _logger.LogInformation(
                "Completed HTTP {Method} {Path} with status code {StatusCode}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.Response.StatusCode);
        }
    }

    private static string ResolveCorrelationId(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue(HttpHeaderNames.CorrelationId, out var headerValue)
            && !string.IsNullOrWhiteSpace(headerValue))
        {
            return headerValue.ToString();
        }

        return Guid.NewGuid().ToString("D");
    }
}