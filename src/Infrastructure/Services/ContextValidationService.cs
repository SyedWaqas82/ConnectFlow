using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Entities;
using ConnectFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Infrastructure.Services;

public class ContextValidationService : IContextValidationService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IContextManager _contextManager;

    public ContextValidationService(IApplicationDbContext dbContext, IContextManager contextManager)
    {
        _dbContext = dbContext;
        _contextManager = contextManager;
    }

    private (int? tenantId, int? userId) GetTenantAndUserId() => (_contextManager.GetCurrentTenantId(), _contextManager.GetCurrentApplicationUserId());

    private bool IsSuperAdminAllowed(bool allowSuperAdmin) => allowSuperAdmin && _contextManager.IsSuperAdmin();

    /// <inheritdoc />
    public async Task<bool> IsCurrentUserFromCurrentTenantHasActiveSubscriptionAsync(bool allowSuperAdmin = true)
    {
        var (tenantId, userId) = GetTenantAndUserId();
        if (!tenantId.HasValue || !userId.HasValue)
            return false;

        if (IsSuperAdminAllowed(allowSuperAdmin))
            return true;

        var isTenantUser = await _dbContext.TenantUsers.AnyAsync(tu => tu.UserId == userId.Value && tu.IsActive);

        if (!isTenantUser)
            return false;

        return await HasActiveSubscriptionAsync();
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
                tur.TenantUser.UserId == userId.Value &&
                tur.TenantUser.IsActive &&
                tur.RoleName == role);
    }

    /// <inheritdoc />
    public async Task<bool> HasActiveSubscriptionAsync()
    {
        return await _dbContext.Subscriptions.AnyAsync(s => s.Status == SubscriptionStatus.Active && (s.CurrentPeriodEndsAt == null || s.CurrentPeriodEndsAt > DateTimeOffset.UtcNow));
    }

    /// <inheritdoc />
    public async Task<Subscription?> GetActiveSubscriptionAsync()
    {
        return await _dbContext.Subscriptions.Where(s => s.Status == SubscriptionStatus.Active && (s.CurrentPeriodEndsAt == null || s.CurrentPeriodEndsAt > DateTimeOffset.UtcNow))
            .OrderByDescending(s => s.CurrentPeriodEndsAt)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<int?> GetDaysLeftInCurrentPeriodAsync()
    {
        var subscription = await GetActiveSubscriptionAsync();
        if (subscription?.CurrentPeriodEndsAt == null)
            return null;

        var daysLeft = (subscription.CurrentPeriodEndsAt.Value - DateTimeOffset.UtcNow).Days;
        return daysLeft > 0 ? daysLeft : 0;
    }

    /// <inheritdoc />
    public async Task<bool> IsInTrialPeriodAsync()
    {
        var subscription = await GetActiveSubscriptionAsync();
        return subscription?.TrialEndsAt > DateTimeOffset.UtcNow;
    }

    /// <inheritdoc />
    public Task<bool> CanAddEntityAsync(LimitValidationType limitValidationType)
    {
        return limitValidationType switch
        {
            LimitValidationType.User => CanAddUserAsync(),
            LimitValidationType.WhatsAppAccount => CanAddSocialAccountAsync(limitValidationType),
            LimitValidationType.FacebookAccount => CanAddSocialAccountAsync(limitValidationType),
            LimitValidationType.InstagramAccount => CanAddSocialAccountAsync(limitValidationType),
            LimitValidationType.Contact => CanAddContactAsync(),
            LimitValidationType.Company => CanAddCompanyAsync(),
            LimitValidationType.Lead => CanAddLeadAsync(),
            _ => Task.FromResult(false)
        };
    }

    private async Task<bool> CanAddUserAsync()
    {
        var subscription = await GetActiveSubscriptionAsync();
        if (subscription == null)
            return false;

        int userCount = await _dbContext.TenantUsers.CountAsync(tu => tu.IsActive);
        return userCount < subscription.UsersLimit;
    }

    private async Task<bool> CanAddSocialAccountAsync(LimitValidationType limitValidationType)
    {
        var subscription = await GetActiveSubscriptionAsync();
        if (subscription == null)
            return false;

        // Validate that active channel accounts do not exceed subscription limits per type and in total
        var channelType = GetChannelType(limitValidationType);
        var channelLimit = GetChannelAccountLimit(limitValidationType, subscription);
        var totalAccountsLimit = subscription.TotalAccountsLimit;

        // total accounts should not exceed than total count and also per channel type count
        var counts = await _dbContext.ChannelAccounts.GroupBy(ca => 1).Select(g => new
        {
            ChannelTypeCount = g.Count(ca => ca.Type == channelType),
            TotalCount = g.Count()
        }).FirstOrDefaultAsync();

        return counts != null && counts.ChannelTypeCount < channelLimit && counts.TotalCount < totalAccountsLimit;
    }

    private ChannelType GetChannelType(LimitValidationType limitValidationType)
    {
        return limitValidationType switch
        {
            LimitValidationType.WhatsAppAccount => ChannelType.WhatsApp,
            LimitValidationType.FacebookAccount => ChannelType.Facebook,
            LimitValidationType.InstagramAccount => ChannelType.Instagram,
            LimitValidationType.TelegramAccount => ChannelType.Telegram,
            _ => throw new ArgumentOutOfRangeException(nameof(limitValidationType), limitValidationType, null)
        };
    }

    private int GetChannelAccountLimit(LimitValidationType limitValidationType, Subscription subscription)
    {
        return limitValidationType switch
        {
            LimitValidationType.WhatsAppAccount => subscription.WhatsAppAccountsLimit,
            LimitValidationType.FacebookAccount => subscription.FacebookAccountsLimit,
            LimitValidationType.InstagramAccount => subscription.InstagramAccountsLimit,
            _ => throw new ArgumentOutOfRangeException(nameof(limitValidationType), limitValidationType, null)
        };
    }

    private async Task<bool> CanAddContactAsync()
    {
        // If you don't have contact limits in subscription, return true
        // Or implement based on your business logic
        return await Task.FromResult(true);
    }

    private async Task<bool> CanAddCompanyAsync()
    {
        // If you don't have company limits in subscription, return true
        // Or implement based on your business logic
        return await Task.FromResult(true);
    }

    private async Task<bool> CanAddLeadAsync()
    {
        // If you don't have lead limits in subscription, return true
        // Or implement based on your business logic
        return await Task.FromResult(true);
    }

    private async Task<bool> CanAddCustomFieldAsync()
    {
        // If you don't have custom field limits in subscription, return true
        // Or implement based on your business logic
        return await Task.FromResult(true);
    }
}