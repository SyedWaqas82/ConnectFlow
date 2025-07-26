using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Services;

namespace ConnectFlow.Web.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        try
        {
            // Skip tenant resolution for static files, health checks, and API docs
            var path = context.Request.Path;
            if (path.StartsWithSegments("/api/specification.json") || path.StartsWithSegments("/health") || path.Value?.EndsWith(".js") == true || path.Value?.EndsWith(".css") == true || path.Value?.EndsWith(".html") == true || path.Value?.EndsWith(".ico") == true)
            {
                await _next(context);
                return;
            }

            // Get the tenant ID from the resolver
            // The resolver will check headers and other sources
            var tenantId = await tenantService.GetCurrentTenantIdAsync();

            if (tenantId.HasValue)
            {
                // Set the tenant ID in the TenantInfo for the current request
                await tenantService.SetCurrentTenantIdAsync(tenantId.Value);
            }
            else
            {
                // Set IsSuperAdmin flag
                TenantInfo.IsSuperAdmin = tenantService.IsSuperAdmin();
            }

            // Call the next middleware
            await _next(context);
        }
        finally
        {
            // Clear the tenant ID after the request is done
            await tenantService.ClearCurrentTenantAsync();
        }
    }
}