using System.Security.Claims;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Infrastructure.Services;

/// <summary>
/// Unified context service that implements all context-related interfaces
/// </summary>
public class UnifiedContextService : IContextManager
{
    private readonly ILogger<UnifiedContextService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _dbContext;

    private int? _currentTenantId { get; set; } = null;
    private int? _applicationUserId { get; set; } = null;
    private Guid? _publicUserId { get; set; } = null;
    private string? _userName { get; set; } = string.Empty;
    private List<string> _roles { get; set; } = new List<string>();
    private bool _isSuperAdmin { get; set; } = false;

    public UnifiedContextService(ILogger<UnifiedContextService> logger, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IApplicationDbContext dbContext)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task InitializeContextAsync(int applicationUserId, int? tenantId)
    {
        _logger.LogInformation("Initializing context for application user ID {UserId} and tenant ID {TenantId}", applicationUserId, tenantId);

        // Clear existing context
        ClearContext();

        // Set user context from database
        var user = await _userManager.FindByIdAsync(applicationUserId.ToString());
        if (user != null)
        {
            _applicationUserId = user.Id;
            _publicUserId = user.PublicId;
            _userName = user.UserName;

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);
            _roles = roles.ToList();
            _isSuperAdmin = roles.Contains(Roles.SuperAdmin);

            _logger.LogDebug("Set user context: AppId={AppId}, PublicId={PublicId}, IsSuperAdmin={IsSuperAdmin}", user.Id, user.PublicId, _isSuperAdmin);

            // Set tenant context
            if (tenantId.HasValue)
            {
                _currentTenantId = tenantId;
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
            _applicationUserId = user.Id;
            _publicUserId = user.PublicId;
            _userName = user.UserName;

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);
            _roles = roles.ToList();
            _isSuperAdmin = roles.Contains(Roles.SuperAdmin);

            _logger.LogDebug("Set user context: AppId={AppId}, PublicId={PublicId}, IsSuperAdmin={IsSuperAdmin}", user.Id, user.PublicId, _isSuperAdmin);

            // Skip tenant resolution for SuperAdmin users
            if (_isSuperAdmin)
            {
                _logger.LogDebug("Super admin user - skipping tenant context");
                return;
            }

            // Find default tenant for this user
            var defaultTenant = await _dbContext.TenantUsers
                .Where(tu => tu.UserId == user.Id && tu.IsActive)
                .OrderByDescending(tu => tu.JoinedAt)
                .FirstOrDefaultAsync();

            if (defaultTenant != null)
            {
                _currentTenantId = defaultTenant.TenantId;
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
            _applicationUserId = applicationUserId;

        if (publicUserId.HasValue)
            _publicUserId = publicUserId;

        if (userName != null)
            _userName = userName;

        if (roles != null)
            _roles = roles;

        _isSuperAdmin = isSuperAdmin;

        // Set tenant info
        if (tenantId.HasValue)
            _currentTenantId = tenantId;
    }

    public void ClearContext()
    {
        _logger.LogInformation("Clearing user and tenant context");

        _applicationUserId = null;
        _publicUserId = null;
        _userName = null;
        _roles = new List<string>();
        _isSuperAdmin = false;

        _currentTenantId = null;
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
        var staticAppId = _applicationUserId;
        _logger.LogTrace("Retrieved application user ID {AppId} from static context", staticAppId);

        return staticAppId;
    }

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
        var staticUserId = _publicUserId;
        _logger.LogTrace("Retrieved user ID {UserId} from static context", staticUserId);

        return staticUserId;
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
        var staticUserName = _userName;
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
        var staticRoles = _roles;
        _logger.LogTrace("Retrieved roles {Roles} from static context", string.Join(", ", staticRoles ?? new List<string>()));

        return staticRoles ?? new List<string>();
    }

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
        var staticTenantId = _currentTenantId;
        _logger.LogTrace("Retrieved tenant ID {TenantId} from static context", staticTenantId);

        return staticTenantId;
    }

    public bool IsSuperAdmin()
    {
        var roles = GetCurrentUserRoles();
        var isSuperAdmin = roles.Contains(Roles.SuperAdmin) || _isSuperAdmin;
        _logger.LogTrace("Current user is super admin: {IsSuperAdmin}", isSuperAdmin);

        return isSuperAdmin;
    }

    public bool IsInRole(string role)
    {
        return GetCurrentUserRoles().Contains(role);
    }
}