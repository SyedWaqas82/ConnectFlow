using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Entities;
using ConnectFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Infrastructure.Services;

public class ContextValidationService : IContextValidationService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentTenantService _tenantService;
    private readonly ICurrentUserService _currentUserService;

    public ContextValidationService(
        IApplicationDbContext dbContext,
        ICurrentTenantService tenantService,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _tenantService = tenantService;
        _currentUserService = currentUserService;
    }

    private (int? tenantId, int? userId) GetTenantAndUserId() => (_tenantService.GetCurrentTenantId(), _currentUserService.GetCurrentApplicationUserId());

    private bool IsSuperAdminAllowed(bool allowSuperAdmin) => allowSuperAdmin && _currentUserService.IsSuperAdmin();

    /// <inheritdoc />
    public async Task<bool> IsCurrentUserFromCurrentTenantAsync(bool allowSuperAdmin = true)
    {
        var (tenantId, userId) = GetTenantAndUserId();
        if (!tenantId.HasValue || !userId.HasValue)
            return false;

        if (IsSuperAdminAllowed(allowSuperAdmin))
            return true;

        return await _dbContext.TenantUsers
            .AnyAsync(tu => tu.TenantId == tenantId.Value && tu.UserId == userId.Value && tu.IsActive);
    }

    /// <inheritdoc />
    public async Task<bool> IsCurrentUserFromCurrentTenantHasActiveSubscriptionAsync(bool allowSuperAdmin = true)
    {
        var (tenantId, userId) = GetTenantAndUserId();
        if (!tenantId.HasValue || !userId.HasValue)
            return false;

        if (IsSuperAdminAllowed(allowSuperAdmin))
            return true;

        var isTenantUser = await _dbContext.TenantUsers.AnyAsync(tu => tu.TenantId == tenantId.Value && tu.UserId == userId.Value && tu.IsActive);

        if (!isTenantUser)
            return false;

        return await HasActiveSubscriptionAsync(tenantId.Value);
    }

    /// <inheritdoc />
    public async Task<bool> IsCurrentUserFromCurrentTenantHasRoleAsync(string role, bool allowSuperAdmin = true)
    {
        var (tenantId, userId) = GetTenantAndUserId();
        if (!tenantId.HasValue || !userId.HasValue)
            return false;

        if (IsSuperAdminAllowed(allowSuperAdmin))
            return true;

        return await _dbContext.TenantUserRoles.Include(tur => tur.TenantUser).AnyAsync(tur =>
                tur.TenantUser.TenantId == tenantId.Value &&
                tur.TenantUser.UserId == userId.Value &&
                tur.TenantUser.IsActive &&
                tur.IsActive &&
                tur.RoleName == role);
    }

    /// <inheritdoc />
    public async Task<bool> HasActiveSubscriptionAsync(int tenantId)
    {
        return await _dbContext.Subscriptions.AnyAsync(s => s.TenantId == tenantId && s.Status == SubscriptionStatus.Active && (s.CurrentPeriodEndsAt == null || s.CurrentPeriodEndsAt > DateTimeOffset.UtcNow));
    }

    /// <inheritdoc />
    public async Task<Subscription?> GetActiveSubscriptionAsync(int tenantId)
    {
        return await _dbContext.Subscriptions.Where(s => s.TenantId == tenantId && s.Status == SubscriptionStatus.Active && (s.CurrentPeriodEndsAt == null || s.CurrentPeriodEndsAt > DateTimeOffset.UtcNow))
            .OrderByDescending(s => s.CurrentPeriodEndsAt)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<int?> GetDaysLeftInCurrentPeriodAsync(int tenantId)
    {
        var subscription = await GetActiveSubscriptionAsync(tenantId);
        if (subscription?.CurrentPeriodEndsAt == null)
            return null;

        var daysLeft = (subscription.CurrentPeriodEndsAt.Value - DateTimeOffset.UtcNow).Days;
        return daysLeft > 0 ? daysLeft : 0;
    }

    /// <inheritdoc />
    public async Task<bool> IsInTrialPeriodAsync(int tenantId)
    {
        var subscription = await GetActiveSubscriptionAsync(tenantId);
        return subscription?.TrialEndsAt > DateTimeOffset.UtcNow;
    }
}