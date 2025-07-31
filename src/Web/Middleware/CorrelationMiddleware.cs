using System.Diagnostics;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace ConnectFlow.Web.Middleware;

public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationHeaderName = "X-Correlation-ID";

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract or create correlation ID
        var correlationId = GetOrCreateCorrelationId(context);
        context.Items["CorrelationId"] = correlationId;

        // Add to response headers
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append(CorrelationHeaderName, correlationId);
            return Task.CompletedTask;
        });

        // Create an Activity for tracing
        using var activity = ConnectFlowTracing.ActivitySource.StartActivity("HttpRequest");
        activity?.SetTag("correlation.id", correlationId);

        // Add to logging context for Serilog
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationHeaderName, out var correlationId) && !string.IsNullOrEmpty(correlationId))
        {
            return correlationId.ToString();
        }

        return Guid.NewGuid().ToString();
    }
}

/// <summary>
/// ActivitySource for distributed tracing
/// </summary>
public static class ConnectFlowTracing
{
    public static readonly ActivitySource ActivitySource = new("ConnectFlow");
}

/// <summary>
/// Extension methods for the Infrastructure middleware components.
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds the correlation middleware to the request pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseCorrelationIdMapping(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationMiddleware>();
    }
}

