using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Infrastructure.Services;

public class TenantLimitsService : ITenantLimitsService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISubscriptionService _subscriptionService;

    public TenantLimitsService(IApplicationDbContext dbContext, ISubscriptionService subscriptionService)
    {
        _dbContext = dbContext;
        _subscriptionService = subscriptionService;
    }

    public async Task<bool> CanAddEntityAsync(int tenantId, EntityType entityType)
    {
        var activeSubscription = await _subscriptionService.GetActiveSubscriptionAsync(tenantId);
        if (activeSubscription == null) return false;

        // Check limits based on entity type
        if (entityType == EntityType.Lead)
        {
            int leadCount = await _dbContext.Leads.CountAsync(l => l.TenantId == tenantId);
            return leadCount < activeSubscription.LeadLimit;
        }
        else if (entityType == EntityType.Contact)
        {
            int contactCount = await _dbContext.Contacts.CountAsync(c => c.TenantId == tenantId);
            return contactCount < activeSubscription.ContactLimit;
        }
        else if (entityType == EntityType.Company)
        {
            int companyCount = await _dbContext.Companies.CountAsync(c => c.TenantId == tenantId);
            return companyCount < activeSubscription.CompanyLimit;
        }
        else if (entityType == EntityType.CustomField)
        {
            int customFieldCount = await _dbContext.CustomFields.CountAsync(cf => cf.TenantId == tenantId);
            return customFieldCount < activeSubscription.CustomFieldLimit;
        }

        // If entity type is not limited, allow it
        return true;
    }

    public async Task<bool> CanAddUserAsync(int tenantId)
    {
        var activeSubscription = await _subscriptionService.GetActiveSubscriptionAsync(tenantId);
        if (activeSubscription == null) return false;

        int userCount = await _dbContext.TenantUsers.CountAsync(tu => tu.TenantId == tenantId && tu.IsActive);
        return userCount < activeSubscription.UserLimit;
    }

    public async Task<bool> CanUseAiTokensAsync(int tenantId, int tokenCount)
    {
        var activeSubscription = await _subscriptionService.GetActiveSubscriptionAsync(tenantId);
        if (activeSubscription == null) return false;

        // Get the current month's token usage
        var startOfMonth = new DateTimeOffset(DateTimeOffset.UtcNow.Year, DateTimeOffset.UtcNow.Month, 1, 0, 0, 0, TimeSpan.Zero);

        int currentMonthUsage = await _dbContext.AIUsages
            .Where(a => a.TenantId == tenantId && a.Created >= startOfMonth)
            .SumAsync(a => a.InputTokens + a.OutputTokens);

        int limit = activeSubscription.MonthlyAITokenLimit + activeSubscription.AdditionalAITokens;

        return currentMonthUsage + tokenCount <= limit;
    }
}