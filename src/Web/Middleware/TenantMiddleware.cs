using ConnectFlow.Application.Common.Interfaces;

namespace ConnectFlow.Web.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService, IUserService currentUserService)
    {
        try
        {
            // Get the tenant ID from the resolver
            // The resolver will check headers and other sources
            var tenantId = await tenantService.GetCurrentTenantIdAsync();

            if (tenantId.HasValue)
            {
                // Set the tenant ID in the TenantInfo for the current request
                await tenantService.SetCurrentTenantIdAsync(tenantId.Value);
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