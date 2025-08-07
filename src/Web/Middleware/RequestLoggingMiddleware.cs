using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Serilog.Context;

namespace ConnectFlow.Web.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly DiagnosticListener _diagnosticListener;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, DiagnosticListener diagnosticListener)
    {
        _next = next;
        _logger = logger;
        _diagnosticListener = diagnosticListener;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Don't log metrics endpoint to avoid noise
        if (context.Request.Path.StartsWithSegments("/metrics"))
        {
            await _next(context);
            return;
        }

        // Start timing
        var sw = Stopwatch.StartNew();

        // try
        // {
        // Extract client IP
        var clientIp = GetClientIp(context);

        // Extract client ID from JWT token
        var clientId = GetClientIdFromToken(context);

        // Extract tenant ID from request header
        var tenantId = GetTenantId(context);

        // HTTP Method and Target Endpoint
        var method = context.Request.Method;
        var path = context.Request.Path.Value;
        var queryString = context.Request.QueryString.Value;

        // Enrich the log context with request details
        using (LogContext.PushProperty("ClientIP", clientIp))
        using (LogContext.PushProperty("ClientId", clientId))
        using (LogContext.PushProperty("TenantId", tenantId))
        using (LogContext.PushProperty("HttpMethod", method))
        using (LogContext.PushProperty("RequestPath", path))
        using (LogContext.PushProperty("QueryString", queryString))
        {
            // Log request starting
            _logger.LogInformation("HTTP Request: [Tenant: {TenantId}] {HttpMethod} {RequestPath}{QueryString}", tenantId, method, path, queryString);

            // Call the next middleware in the pipeline
            await _next(context);

            // Stop timing
            sw.Stop();

            // Log response details
            var statusCode = context.Response.StatusCode;
            var responseTimeMs = sw.ElapsedMilliseconds;

            using (LogContext.PushProperty("StatusCode", statusCode))
            using (LogContext.PushProperty("ResponseTimeMs", responseTimeMs))
            {
                // Categorize response by status code
                if (statusCode >= 500)
                {
                    _logger.LogError("HTTP Response: [Tenant: {TenantId}] {StatusCode} in {ResponseTimeMs}ms for {HttpMethod} {RequestPath}",
                        tenantId, statusCode, responseTimeMs, method, path);
                }
                else if (statusCode >= 400)
                {
                    _logger.LogWarning("HTTP Response: [Tenant: {TenantId}] {StatusCode} in {ResponseTimeMs}ms for {HttpMethod} {RequestPath}",
                        tenantId, statusCode, responseTimeMs, method, path);
                }
                else
                {
                    _logger.LogInformation("HTTP Response: [Tenant: {TenantId}] {StatusCode} in {ResponseTimeMs}ms for {HttpMethod} {RequestPath}",
                        tenantId, statusCode, responseTimeMs, method, path);
                }
            }
        }
        // }
        // catch (Exception ex)
        // {
        //     // Log middleware exception
        //     _logger.LogError(ex, "Error in request logging middleware");
        //     await _next(context);
        // }
    }

    private string GetClientIp(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString();

        // Check for forwarded headers (e.g., when behind a proxy)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            // X-Forwarded-For may contain multiple IPs - take the first one
            clientIp = forwardedFor.Split(',').FirstOrDefault()?.Trim();
        }

        return clientIp ?? "unknown";
    }

    private string GetTenantId(HttpContext context)
    {
        try
        {
            // Try to get tenant ID from the standard header
            var tenantHeaderName = context.RequestServices
                .GetService<IConfiguration>()?["TenantSettings:HeaderName"] ?? "X-Tenant-Id";

            if (context.Request.Headers.TryGetValue(tenantHeaderName, out var tenantId) && !string.IsNullOrEmpty(tenantId))
            {
                return tenantId.ToString();
            }

            // Try to get tenant ID from JWT claims if not in header
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var tenantClaim = context.User.Claims.FirstOrDefault(c =>
                    c.Type == "tenant_id" ||
                    c.Type == "tenant" ||
                    c.Type == "org_id" ||
                    c.Type == "organization");

                if (tenantClaim != null)
                {
                    return tenantClaim.Value;
                }
            }

            return "default";
        }
        catch
        {
            return "unknown";
        }
    }

    private string GetClientIdFromToken(HttpContext context)
    {
        try
        {
            // Try to get client ID from claims
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                // Check for client_id claim (standard for OAuth)
                var clientIdClaim = context.User.Claims.FirstOrDefault(c =>
                    c.Type == "client_id" ||
                    c.Type == "sub" ||
                    c.Type == ClaimTypes.NameIdentifier);

                if (clientIdClaim != null)
                {
                    return clientIdClaim.Value;
                }
            }

            // Try to extract from Authorization header if claim not found
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var jwtHandler = new JwtSecurityTokenHandler();

                if (jwtHandler.CanReadToken(token))
                {
                    var jwtToken = jwtHandler.ReadJwtToken(token);

                    // Look for client_id or sub claim
                    var clientIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                        c.Type == "client_id" ||
                        c.Type == "sub" ||
                        c.Type == ClaimTypes.NameIdentifier);

                    if (clientIdClaim != null)
                    {
                        return clientIdClaim.Value;
                    }
                }
            }

            return "anonymous";
        }
        catch
        {
            // In case of any error parsing the token
            return "unknown";
        }
    }
}

/// <summary>
/// Extension methods for the request logging middleware.
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds the request logging middleware to the HTTP pipeline.
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}