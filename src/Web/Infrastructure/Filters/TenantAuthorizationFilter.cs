using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Web.Infrastructure.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequiresTenantAttribute : Attribute, IAsyncAuthorizationFilter
{
    public RequiresTenantAttribute(bool allowSuperAdmin = true)
    {
        AllowSuperAdmin = allowSuperAdmin;
    }

    public bool AllowSuperAdmin { get; }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var tenantService = context.HttpContext.RequestServices.GetRequiredService<ITenantService>();
        var currentUserService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

        if (!currentUserService.ApplicationUserId.HasValue)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Super admins are always allowed if AllowSuperAdmin is true
        if (AllowSuperAdmin && currentUserService.Roles.Contains(Roles.SuperAdmin))
        {
            return;
        }

        var tenantId = await tenantService.GetCurrentTenantIdAsync();

        if (!tenantId.HasValue)
        {
            context.Result = new ForbidResult();
            return;
        }

        // Check if user belongs to the tenant
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<IApplicationDbContext>();
        var tenantUser = await dbContext.TenantUsers
            .Where(tu => tu.TenantId == tenantId.Value && tu.UserId == currentUserService.ApplicationUserId)
            .FirstOrDefaultAsync();

        if (tenantUser == null || !tenantUser.IsActive)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequiresTenantRoleAttribute : Attribute, IAsyncAuthorizationFilter
{
    public RequiresTenantRoleAttribute(string role, bool allowSuperAdmin = true)
    {
        Role = role;
        AllowSuperAdmin = allowSuperAdmin;
    }

    public string Role { get; }
    public bool AllowSuperAdmin { get; }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var tenantService = context.HttpContext.RequestServices.GetRequiredService<ITenantService>();
        var currentUserService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

        if (!currentUserService.ApplicationUserId.HasValue)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Super admins are always allowed if AllowSuperAdmin is true
        if (AllowSuperAdmin && currentUserService.Roles.Contains(Roles.SuperAdmin))
        {
            return;
        }

        var tenantId = await tenantService.GetCurrentTenantIdAsync();

        if (!tenantId.HasValue)
        {
            context.Result = new ForbidResult();
            return;
        }

        // Check if user belongs to the tenant and has the required role
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<IApplicationDbContext>();
        var tenantUserRoles = await dbContext.TenantUserRoles
            .Include(tur => tur.TenantUser)
            .Where(tur => tur.TenantUser.TenantId == tenantId.Value &&
                tur.TenantUser.UserId == currentUserService.ApplicationUserId &&
                tur.TenantUser.IsActive && tur.IsActive)
            .Select(tur => tur.RoleName)
            .ToListAsync();

        if (!tenantUserRoles.Contains(Role))
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
