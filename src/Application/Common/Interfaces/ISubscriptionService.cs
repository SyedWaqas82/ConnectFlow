using ConnectFlow.Domain.Entities;

namespace ConnectFlow.Application.Common.Interfaces;

public interface ISubscriptionService
{
    /// <summary>
    /// Checks if the tenant has an active subscription
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <returns>True if the tenant has an active subscription, false otherwise</returns>
    Task<bool> HasActiveSubscriptionAsync(int tenantId);

    /// <summary>
    /// Gets the active subscription for the specified tenant
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <returns>The active subscription, or null if none exists</returns>
    Task<Subscription?> GetActiveSubscriptionAsync(int tenantId);

    /// <summary>
    /// Gets the number of days left until the current subscription period ends
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <returns>The number of days left, or null if the subscription doesn't expire</returns>
    Task<int?> GetDaysLeftInCurrentPeriodAsync(int tenantId);

    /// <summary>
    /// Checks if the tenant's subscription is in a trial period
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <returns>True if the tenant's subscription is in a trial period, false otherwise</returns>
    Task<bool> IsInTrialPeriodAsync(int tenantId);
}