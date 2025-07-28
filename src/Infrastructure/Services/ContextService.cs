using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Models;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Infrastructure.Services;

/// <summary>
/// Unified implementation of IContextService for managing user and tenant context
/// Handles context in both HTTP and non-HTTP scenarios
/// </summary>
public class ContextService : IContextService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _dbContext;

    public ContextService(UserManager<ApplicationUser> userManager, IApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Initialize context for a specific application user ID and tenant
    /// </summary>
    public async Task InitializeContextByUserWithTenantAsync(int applicationUserId, int? tenantId)
    {
        // Clear any existing context first
        await ClearContextAsync();

        // Set user info using application ID
        var user = await _userManager.FindByIdAsync(applicationUserId.ToString());

        if (user != null)
        {
            UserInfo.ApplicationUserId = user.Id;
            UserInfo.PublicUserId = user.PublicId;
            UserInfo.UserName = user.UserName;
            UserInfo.Roles = _userManager.GetRolesAsync(user).Result.ToList();
            UserInfo.IsSuperAdmin = UserInfo.IsInRole(Roles.SuperAdmin);
        }

        if (tenantId.HasValue)
        {
            // Set tenant context
            TenantInfo.CurrentTenantId = tenantId;
        }
    }

    /// <summary>
    /// Initialize context for a specific application user with their default tenant
    /// </summary>
    public async Task InitializeContextForUserWithDefaultTenantAsync(int applicationUserId)
    {
        // Clear any existing context first
        await ClearContextAsync();

        // Set user info
        var user = await _userManager.FindByIdAsync(applicationUserId.ToString());

        if (user != null)
        {
            UserInfo.ApplicationUserId = user.Id;
            UserInfo.PublicUserId = user.PublicId;
            UserInfo.UserName = user.UserName;
            UserInfo.Roles = _userManager.GetRolesAsync(user).Result.ToList();

            // Check if SuperAdmin (don't need to set tenant for SuperAdmin)
            if (UserInfo.IsInRole(Roles.SuperAdmin))
            {
                UserInfo.IsSuperAdmin = true;
                return;
            }

            // Set default tenant if not SuperAdmin
            var defaultTenant = await _dbContext.TenantUsers
                .Where(tu => tu.UserId == user.Id && tu.IsActive && tu.Tenant.IsActive)
                .OrderByDescending(tu => tu.JoinedAt)
                .FirstOrDefaultAsync();

            if (defaultTenant != null)
            {
                TenantInfo.CurrentTenantId = defaultTenant.TenantId;
            }
        }
    }

    /// <summary>
    /// Get the current tenant ID in this context
    /// </summary>
    public int? GetCurrentTenantId()
    {
        return TenantInfo.CurrentTenantId;
    }

    /// <summary>
    /// Get the current user name in this context
    /// </summary>
    public string? GetCurrentUserName()
    {
        return UserInfo.UserName;
    }

    /// <summary>
    /// Get the current public user ID in this context
    /// </summary>
    public Guid? GetCurrentPublicUserId()
    {
        return UserInfo.PublicUserId;
    }

    /// <summary>
    /// Get the current application user ID in this context
    /// </summary>
    public int? GetCurrentApplicationUserId()
    {
        return UserInfo.ApplicationUserId;
    }

    /// <summary>
    /// Check if the current context has SuperAdmin privileges
    /// </summary>
    public bool IsSuperAdmin()
    {
        return UserInfo.IsSuperAdmin || UserInfo.IsInRole(Roles.SuperAdmin);
    }

    /// <summary>
    /// Get the roles for the current user in this context
    /// </summary>
    public IList<string> GetCurrentUserRoles()
    {
        return UserInfo.Roles;
    }

    /// <summary>
    /// Check if the current user has a specific role
    /// </summary>
    public bool IsInRole(string role)
    {
        return UserInfo.Roles.Contains(role);
    }

    /// <summary>
    /// Clear all context information
    /// </summary>
    public Task ClearContextAsync()
    {
        // Clear tenant context
        TenantInfo.Clear();

        // Clear user context
        UserInfo.Clear();

        return Task.CompletedTask;
    }
}