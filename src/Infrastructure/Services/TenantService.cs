using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Common.Services;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConnectFlow.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _currentUserService;
    private readonly TenantSettings _tenantSettings;

    public TenantService(IApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IUserService currentUserService, IOptions<TenantSettings> tenantSettings)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
        _tenantSettings = tenantSettings.Value;
    }

    public async Task<int?> GetCurrentTenantIdAsync()
    {
        // First check if we have a tenant ID in the AsyncLocal storage
        // This ensures we can use this in both HTTP and non-HTTP contexts
        if (TenantInfo.CurrentTenantId.HasValue)
        {
            return TenantInfo.CurrentTenantId;
        }

        // Check for tenant ID in HTTP header
        if (_httpContextAccessor.HttpContext != null)
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(_tenantSettings.HeaderName, out var tenantIdHeader))
            {
                if (int.TryParse(tenantIdHeader, out var tenantId))
                {
                    // Store in AsyncLocal for subsequent calls
                    TenantInfo.CurrentTenantId = tenantId;
                    return tenantId;
                }
            }

            // If no tenant ID in header, but user is authenticated, try to get from default tenant
            // Only do this for non-SuperAdmin users to avoid mixing tenants
            if (_currentUserService.ApplicationUserId.HasValue && !_currentUserService.Roles.Contains(Roles.SuperAdmin))
            {
                var defaultTenant = await _dbContext.TenantUsers
                    .Include(tu => tu.Tenant)
                    .Where(tu => tu.UserId == _currentUserService.ApplicationUserId.Value && tu.IsActive && tu.Tenant.IsActive)
                    .OrderByDescending(tu => tu.JoinedAt)
                    .FirstOrDefaultAsync();

                if (defaultTenant != null)
                {
                    // Store in AsyncLocal for subsequent calls
                    TenantInfo.CurrentTenantId = defaultTenant.TenantId;
                    return defaultTenant.TenantId;
                }
            }
        }

        return null;
    }

    public async Task<Tenant?> GetCurrentTenantAsync()
    {
        var tenantId = await GetCurrentTenantIdAsync();
        if (!tenantId.HasValue)
        {
            return null;
        }

        return await _dbContext.Tenants.FindAsync(tenantId.Value);
    }

    public Task SetCurrentTenantIdAsync(int tenantId)
    {
        TenantInfo.CurrentTenantId = tenantId;
        TenantInfo.IsSuperAdmin = IsSuperAdmin();

        return Task.CompletedTask;
    }

    public Task ClearCurrentTenantAsync()
    {
        TenantInfo.CurrentTenantId = null;
        return Task.CompletedTask;
    }

    public bool IsSuperAdmin()
    {
        return _currentUserService.Roles.Contains(Roles.SuperAdmin);
    }
}