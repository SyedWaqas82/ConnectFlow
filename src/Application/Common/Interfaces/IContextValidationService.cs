using ConnectFlow.Domain.Entities;
using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IContextValidationService
{
    /// <summary>
    /// Checks if the current user belongs to the current tenant and has an active subscription
    /// </summary>
    /// <param name="allowSuperAdmin">Whether to allow super admin users</param>
    /// <returns>True if the user belongs to the tenant and has an active subscription, false otherwise</returns>
    /// <remarks> This method checks if the current user is associated with the current tenant and has an active subscription.
    /// If allowSuperAdmin is true, it allows super admin users to pass the check regardless of their subscription status.
    /// </remarks>
    Task<bool> IsCurrentUserFromCurrentTenantHasActiveSubscriptionAsync(bool allowSuperAdmin = true);

    /// <summary>
    /// Checks if the current user belongs to the current tenant and has a specific role
    /// </summary>
    /// <param name="role">The role to check</param>
    /// <param name="allowSuperAdmin">Whether to allow super admin users</param>
    /// <returns>True if the user belongs to the tenant and has the specified role, false otherwise</returns>
    /// <remarks> This method checks if the current user is associated with the current tenant and has the specified role.
    /// If allowSuperAdmin is true, it allows super admin users to pass the check regardless of their role.
    /// </remarks>
    Task<bool> IsCurrentUserFromCurrentTenantHasRoleAsync(string role, bool allowSuperAdmin = true);

    /// <summary>
    /// Checks if the current tenant has an active subscription
    /// </summary>
    /// <returns>True if the tenant has an active subscription, false otherwise</returns>
    Task<bool> HasActiveSubscriptionAsync();

    /// <summary>
    /// Gets the active subscription for the current tenant
    /// </summary>
    /// <returns>The active subscription, or null if none exists</returns>
    Task<Subscription?> GetActiveSubscriptionAsync();

    /// <summary>
    /// Gets the number of days left until the current subscription period ends for the current tenant
    /// </summary>
    /// <returns>The number of days left, or null if the subscription doesn't expire</returns>
    Task<int?> GetDaysLeftInCurrentPeriodAsync();

    /// <summary>
    /// Checks if the current tenant's subscription is in a trial period
    /// </summary>
    /// <returns>True if the current tenant's subscription is in a trial period, false otherwise</returns>
    Task<bool> IsInTrialPeriodAsync();

    /// <summary>
    /// Checks if the current tenant has reached its limit for the specified entity
    /// </summary>
    /// <param name="entityType">The type of entity to check limits for</param>
    /// <returns>True if the tenant has not reached its limit, false otherwise</returns>
    Task<bool> CanAddEntityAsync(EntityType entityType);
}