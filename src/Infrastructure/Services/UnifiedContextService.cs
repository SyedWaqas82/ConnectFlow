using System.Security.Claims;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Infrastructure.Common.Models;
using ConnectFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Infrastructure.Services;

/// <summary>
/// Unified context service that implements all context-related interfaces
/// </summary>
public class UnifiedContextService : ICurrentUserService, ICurrentTenantService, IContextManager
{
    private readonly ILogger<UnifiedContextService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _dbContext;

    public UnifiedContextService(ILogger<UnifiedContextService> logger, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IApplicationDbContext dbContext)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    #region ICurrentUserService Implementation

    public Guid? GetCurrentUserId()
    {
        // Try HTTP context first
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                _logger.LogTrace("Retrieved user ID {UserId} from HTTP claims", userId);
                return userId;
            }
        }

        // Fall back to static context
        var staticUserId = UserInfo.PublicUserId;
        _logger.LogTrace("Retrieved user ID {UserId} from static context", staticUserId);

        return staticUserId;
    }

    public int? GetCurrentApplicationUserId()
    {
        // Try HTTP context first
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var appIdString = httpContext.User.FindFirstValue(ClaimTypes.Sid);
            if (int.TryParse(appIdString, out int appId))
            {
                _logger.LogTrace("Retrieved application user ID {AppId} from HTTP claims", appId);
                return appId;
            }
        }

        // Fall back to static context
        var staticAppId = UserInfo.ApplicationUserId;
        _logger.LogTrace("Retrieved application user ID {AppId} from static context", staticAppId);

        return staticAppId;
    }

    public string? GetCurrentUserName()
    {
        // Try HTTP context first
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userName = httpContext.User.FindFirstValue(ClaimTypes.Name);
            if (!string.IsNullOrEmpty(userName))
            {
                _logger.LogTrace("Retrieved username {Username} from HTTP claims", userName);
                return userName;
            }
        }

        // Fall back to static context
        var staticUserName = UserInfo.UserName;
        _logger.LogTrace("Retrieved username {Username} from static context", staticUserName);

        return staticUserName;
    }

    public List<string> GetCurrentUserRoles()
    {
        // Try HTTP context first
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var rolesString = httpContext.User.FindFirstValue(ClaimTypes.Role);
            if (!string.IsNullOrEmpty(rolesString))
            {
                var roles = rolesString.Split(',').ToList();
                _logger.LogTrace("Retrieved roles {Roles} from HTTP claims", string.Join(", ", roles));
                return roles;
            }
        }

        // Fall back to static context
        var staticRoles = UserInfo.Roles;
        _logger.LogTrace("Retrieved roles {Roles} from static context", string.Join(", ", staticRoles));

        return staticRoles;
    }

    public bool IsSuperAdmin()
    {
        var roles = GetCurrentUserRoles();
        var isSuperAdmin = roles.Contains(Roles.SuperAdmin) || UserInfo.IsSuperAdmin;
        _logger.LogTrace("Current user is super admin: {IsSuperAdmin}", isSuperAdmin);
        return isSuperAdmin;
    }

    #endregion

    #region ICurrentTenantService Implementation

    public int? GetCurrentTenantId()
    {
        // Try HTTP context header first
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            // Get tenant settings
            var tenantHeaderName = "X-Tenant-ID"; // Default header name

            // Try to get tenant header from the request
            if (httpContext.Request.Headers.TryGetValue(tenantHeaderName, out var tenantIdHeader))
            {
                if (int.TryParse(tenantIdHeader, out var tenantId))
                {
                    _logger.LogTrace("Retrieved tenant ID {TenantId} from HTTP header", tenantId);
                    return tenantId;
                }
            }
        }

        // Fall back to static context
        var staticTenantId = TenantInfo.CurrentTenantId;
        _logger.LogTrace("Retrieved tenant ID {TenantId} from static context", staticTenantId);

        return staticTenantId;
    }

    #endregion

    #region IContextManager Implementation

    public async Task InitializeContextAsync(int applicationUserId, int? tenantId)
    {
        _logger.LogInformation("Initializing context for application user ID {UserId} and tenant ID {TenantId}", applicationUserId, tenantId);

        // Clear existing context
        ClearContext();

        // Set user context from database
        var user = await _userManager.FindByIdAsync(applicationUserId.ToString());
        if (user != null)
        {
            UserInfo.ApplicationUserId = user.Id;
            UserInfo.PublicUserId = user.PublicId;
            UserInfo.UserName = user.UserName;

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);
            UserInfo.Roles = roles.ToList();
            UserInfo.IsSuperAdmin = roles.Contains(Roles.SuperAdmin);

            _logger.LogDebug("Set user context: AppId={AppId}, PublicId={PublicId}, IsSuperAdmin={IsSuperAdmin}", user.Id, user.PublicId, UserInfo.IsSuperAdmin);

            // Set tenant context
            if (tenantId.HasValue)
            {
                TenantInfo.CurrentTenantId = tenantId;
                _logger.LogDebug("Set tenant context: TenantId={TenantId}", tenantId);
            }
        }
        else
        {
            _logger.LogWarning("Failed to initialize context - user with ID {UserId} not found", applicationUserId);
        }
    }

    public async Task InitializeContextWithDefaultTenantAsync(int applicationUserId)
    {
        _logger.LogInformation("Initializing context with default tenant for application user ID {UserId}", applicationUserId);

        // Clear existing context
        ClearContext();

        // Set user context from database
        var user = await _userManager.FindByIdAsync(applicationUserId.ToString());
        if (user != null)
        {
            UserInfo.ApplicationUserId = user.Id;
            UserInfo.PublicUserId = user.PublicId;
            UserInfo.UserName = user.UserName;

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);
            UserInfo.Roles = roles.ToList();
            UserInfo.IsSuperAdmin = roles.Contains(Roles.SuperAdmin);

            _logger.LogDebug("Set user context: AppId={AppId}, PublicId={PublicId}, IsSuperAdmin={IsSuperAdmin}", user.Id, user.PublicId, UserInfo.IsSuperAdmin);

            // Skip tenant resolution for SuperAdmin users
            if (UserInfo.IsSuperAdmin)
            {
                _logger.LogDebug("Super admin user - skipping tenant context");
                return;
            }

            // Find default tenant for this user
            var defaultTenant = await _dbContext.TenantUsers
                .Where(tu => tu.UserId == user.Id && tu.IsActive && tu.Tenant.IsActive)
                .OrderByDescending(tu => tu.JoinedAt)
                .FirstOrDefaultAsync();

            if (defaultTenant != null)
            {
                TenantInfo.CurrentTenantId = defaultTenant.TenantId;
                _logger.LogDebug("Set default tenant context: TenantId={TenantId}", defaultTenant.TenantId);
            }
            else
            {
                _logger.LogWarning("No active tenant found for user {UserId}", applicationUserId);
            }
        }
        else
        {
            _logger.LogWarning("Failed to initialize context - user with ID {UserId} not found", applicationUserId);
        }
    }

    public void SetContext(int? applicationUserId, Guid? publicUserId, string? userName, List<string>? roles, bool isSuperAdmin, int? tenantId)
    {
        _logger.LogInformation("Manually setting context: AppUserId={AppUserId}, PublicId={PublicId}, UserName={UserName}, " + "IsSuperAdmin={IsSuperAdmin}, TenantId={TenantId}", applicationUserId, publicUserId, userName, isSuperAdmin, tenantId);

        // Set user info
        if (applicationUserId.HasValue)
            UserInfo.ApplicationUserId = applicationUserId;

        if (publicUserId.HasValue)
            UserInfo.PublicUserId = publicUserId;

        if (userName != null)
            UserInfo.UserName = userName;

        if (roles != null)
            UserInfo.Roles = roles;

        UserInfo.IsSuperAdmin = isSuperAdmin;

        // Set tenant info
        if (tenantId.HasValue)
            TenantInfo.CurrentTenantId = tenantId;
    }

    public void ClearContext()
    {
        _logger.LogInformation("Clearing user and tenant context");
        UserInfo.Clear();
        TenantInfo.Clear();
    }

    public bool IsInRole(string role)
    {
        return GetCurrentUserRoles().Contains(role);
    }

    #endregion
}