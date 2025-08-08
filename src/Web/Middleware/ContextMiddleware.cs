using System.Security.Claims;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Infrastructure.Common.Models;
using Microsoft.Extensions.Options;

namespace ConnectFlow.Web.Middleware;

public class ContextMiddleware
{
    private readonly RequestDelegate _next;

    public ContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IContextService contextService, IOptions<TenantSettings> tenantSettings)
    {
        try
        {
            // Skip tenant resolution for static files, health checks, and API docs
            var path = context.Request.Path;
            if (path.StartsWithSegments("/api/specification.json") || path.StartsWithSegments("/swagger/v1/swagger.json") || path.StartsWithSegments("/health") || path.StartsWithSegments("/metrics") == true || path.Value?.EndsWith(".js") == true || path.Value?.EndsWith(".css") == true || path.Value?.EndsWith(".html") == true || path.Value?.EndsWith(".ico") == true)
            {
                await _next(context);
                return;
            }

            var tenantId = GetCurrentTenantIdAsync(context, tenantSettings.Value);
            var userInfo = GetCurrentUserInfo(context);

            if (userInfo.ApplicationUserId.HasValue)
            {
                // Initialize context with user and tenant information
                await contextService.InitializeContextByUserWithTenantAsync(userInfo.ApplicationUserId.Value, tenantId);
            }
            else
            {
                // If no user is authenticated, clear the context
                await contextService.ClearContextAsync();
            }

            // Call the next middleware
            await _next(context);
        }
        finally
        {
            // We clear the context after the request is done
            await contextService.ClearContextAsync();
        }
    }

    /// <summary>
    /// Initialize user context from HTTP context
    /// This happens earlier in the request pipeline to ensure tenant filtering works properly
    /// </summary>
    private (int? ApplicationUserId, Guid? PublicUserId, string? User, List<string> Roles, bool IsSuperAdmin) GetCurrentUserInfo(HttpContext context)
    {
        // Only set values from HTTP context if authenticated
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // Set ApplicationUserId
            string? appIdStr = context.User.FindFirstValue(ClaimTypes.Sid);
            int.TryParse(appIdStr, out int applicationUserId);

            // Set PublicUserId
            string? pubIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(pubIdStr, out Guid publicUserId);

            // Set UserName
            var userName = context.User.FindFirstValue(ClaimTypes.Name);

            // Set Roles
            string userRoles = context.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            // Create a new list for roles instead of using UserInfo.Roles
            var roles = new List<string>();
            if (!string.IsNullOrEmpty(userRoles))
            {
                roles = userRoles.Split(",").ToList();
            }

            bool isSuperAdmin = userRoles.Contains(Roles.SuperAdmin);

            return (applicationUserId, publicUserId, userName, roles, isSuperAdmin);
        }

        return (null, null, null, new List<string>(), false);
    }

    private int? GetCurrentTenantIdAsync(HttpContext context, TenantSettings tenantSettings)
    {

        if (context.Request.Headers.TryGetValue(tenantSettings.HeaderName, out var tenantIdHeader))
        {
            int.TryParse(tenantIdHeader, out var tenantId);

            return tenantId;
        }

        return null;
    }
}

// create an extension method to use the middleware easily
public static class ContextMiddlewareExtensions
{
    public static IApplicationBuilder UseContextSettings(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ContextMiddleware>();
    }
}