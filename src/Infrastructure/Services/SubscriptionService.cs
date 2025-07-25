using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Entities;
using ConnectFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Infrastructure.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IApplicationDbContext _dbContext;

    public SubscriptionService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasActiveSubscriptionAsync(int tenantId)
    {
        return await _dbContext.Subscriptions.AnyAsync(s => s.TenantId == tenantId && s.Status == SubscriptionStatus.Active && (s.CurrentPeriodEndsAt == null || s.CurrentPeriodEndsAt > DateTimeOffset.UtcNow));
    }

    public async Task<Subscription?> GetActiveSubscriptionAsync(int tenantId)
    {
        return await _dbContext.Subscriptions
            .Where(s => s.TenantId == tenantId && s.Status == SubscriptionStatus.Active && (s.CurrentPeriodEndsAt == null || s.CurrentPeriodEndsAt > DateTimeOffset.UtcNow))
            .OrderByDescending(s => s.CurrentPeriodEndsAt)
            .FirstOrDefaultAsync();
    }

    public async Task<int?> GetDaysLeftInCurrentPeriodAsync(int tenantId)
    {
        var subscription = await GetActiveSubscriptionAsync(tenantId);

        if (subscription?.CurrentPeriodEndsAt == null)
        {
            return null; // Subscription doesn't expire
        }

        var daysLeft = (subscription.CurrentPeriodEndsAt.Value - DateTimeOffset.UtcNow).Days;
        return daysLeft > 0 ? daysLeft : 0;
    }

    public async Task<bool> IsInTrialPeriodAsync(int tenantId)
    {
        var subscription = await GetActiveSubscriptionAsync(tenantId);

        if (subscription == null || subscription.TrialEndsAt == null)
        {
            return false; // No subscription or no trial period
        }

        return subscription.TrialEndsAt > DateTimeOffset.UtcNow;
    }
}