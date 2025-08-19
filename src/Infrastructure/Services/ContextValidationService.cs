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
    public Task<bool> CanAddEntityAsync(EntityType entityType)
    {
        return entityType switch
        {
            EntityType.User => CanAddUserAsync(),
            EntityType.WhatsAppAccount => CanAddWhatsAppAccountAsync(),
            EntityType.Account => CanAddAccountAsync(),
            EntityType.Contact => CanAddContactAsync(),
            EntityType.Company => CanAddCompanyAsync(),
            EntityType.Lead => CanAddLeadAsync(),
            EntityType.CustomField => CanAddCustomFieldAsync(),
            _ => Task.FromResult(false)
        };
    }

    private async Task<bool> CanAddUserAsync()
    {
        var subscription = await GetActiveSubscriptionAsync();
        if (subscription == null)
            return false;

        int userCount = await _dbContext.TenantUsers.CountAsync(tu => tu.IsActive);
        return userCount < subscription.UserLimit;
    }

    private async Task<bool> CanAddWhatsAppAccountAsync()
    {
        var subscription = await GetActiveSubscriptionAsync();
        if (subscription == null)
            return false;

        // Count WhatsApp channel accounts - assuming you have a ChannelAccount entity with Type WhatsApp
        // Replace this with actual entity counting when ChannelAccount entity is available
        int whatsAppCount = 0; // await _dbContext.ChannelAccounts.CountAsync(ca => ca.Type == ChannelType.WhatsApp);

        return whatsAppCount < subscription.WhatsAppAccountLimit;
    }

    private async Task<bool> CanAddAccountAsync()
    {
        var subscription = await GetActiveSubscriptionAsync();
        if (subscription == null)
            return false;

        // Count total accounts - assuming you have account entities
        // Replace this with actual entity counting when account entities are available
        int accountCount = 0; // await _dbContext.ChannelAccounts.CountAsync();

        return accountCount < subscription.TotalAccountLimit;
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