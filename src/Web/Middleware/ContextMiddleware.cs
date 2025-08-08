using ConnectFlow.Application.Common.Interfaces;

namespace ConnectFlow.Web.Middleware;

public class ContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ContextMiddleware> _logger;

    public ContextMiddleware(RequestDelegate next, ILogger<ContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService, ICurrentTenantService currentTenantService)
    {
        try
        {
            // Initialize context from HTTP claims first
            var userId = currentUserService.GetCurrentUserId();
            var applicationUserId = currentUserService.GetCurrentApplicationUserId();
            var tenantId = currentTenantService.GetCurrentTenantId();

            _logger.LogDebug("Context from HTTP request: UserId={UserId}, ApplicationUserId={ApplicationUserId}, TenantId={TenantId}", userId, applicationUserId, tenantId);

            // Set the context - we don't need to manually set it as the UnifiedContextService already reads from HTTP claims
            // This middleware ensures context is available throughout the request

            // Process the request
            await _next(context);

            // Don't clear context after request - this enables async operations to still access context
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ContextMiddleware");
            throw;
        }
    }
}

// Extension method for registering the middleware
public static class ContextMiddlewareExtensions
{
    public static IApplicationBuilder UseContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ContextMiddleware>();
    }
}
