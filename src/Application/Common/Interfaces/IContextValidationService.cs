using ConnectFlow.Domain.Entities;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IContextValidationService
{
    /// <summary>
    /// Checks if the current user belongs to the current tenant
    /// </summary>
    /// <param name="allowSuperAdmin">Whether to allow super admin users</param>
    /// <returns>True if the user belongs to the tenant, false otherwise</returns>
    /// <remarks> This method checks if the current user is associated with the current tenant.
    /// If allowSuperAdmin is true, it allows super admin users to pass the check regardless of their tenant association.
    /// </remarks>
    Task<bool> IsCurrentUserFromCurrentTenantAsync(bool allowSuperAdmin = true);

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