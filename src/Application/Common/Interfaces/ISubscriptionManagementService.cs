using ConnectFlow.Domain.Entities;
using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Application.Common.Interfaces;

/// <summary>
/// Provides methods for managing subscriptions, enforcing limits, handling entity suspension/reactivation, and orchestrating domain events.
/// </summary>
public interface ISubscriptionManagementService
{
    #region Subscription Management

    /// <summary>
    /// Retrieves the active subscription for the specified tenant.
    /// </summary>
    Task<Subscription?> GetActiveSubscriptionAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the specified tenant has an active subscription.
    /// </summary>
    Task<bool> HasActiveSubscriptionAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the subscription history for the specified tenant.
    /// </summary>
    Task<List<Subscription>> GetSubscriptionHistoryAsync(int tenantId, CancellationToken cancellationToken = default);

    #endregion

    #region Current Context Operations

    /// <summary>
    /// Determines whether the current user's tenant has an active subscription.
    /// </summary>
    Task<bool> IsCurrentUserFromCurrentTenantHasActiveSubscriptionAsync(bool allowSuperAdmin = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the current user has the specified role in the current tenant.
    /// </summary>
    Task<bool> IsCurrentUserFromCurrentTenantHasRoleAsync(string role, bool allowSuperAdmin = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the current tenant can add the specified entity type based on plan limits.
    /// </summary>
    Task<(bool CanAdd, int CurrentCount, int MaxCount)> CanAddEntityAsync(LimitValidationType limitValidationType, CancellationToken cancellationToken = default);

    #endregion

    #region Plan Limits Validation

    /// <summary>
    /// Determines whether the specified tenant can add a user based on plan limits.
    /// </summary>
    Task<(bool CanAdd, int CurrentCount, int MaxCount)> CanAddUserAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the specified tenant can add a channel of the given type based on plan limits.
    /// </summary>
    Task<(bool CanAdd, int CurrentCount, int MaxCount)> CanAddChannelAsync(int tenantId, ChannelType channelType, CancellationToken cancellationToken = default);

    #endregion

    #region Usage Tracking

    /// <summary>
    /// Retrieves the current user count for the specified tenant.
    /// </summary>
    Task<int> GetCurrentUserCountAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the channel counts for the specified tenant.
    /// </summary>
    Task<List<(ChannelType Type, int ChannelCount)>> GetChannelsCountAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves usage statistics for the specified tenant.
    /// </summary>
    Task<Dictionary<string, int>> GetUsageStatisticsAsync(int tenantId, CancellationToken cancellationToken = default);

    #endregion

    // #region Entity Suspension/Reactivation (Generic ISuspendibleEntity Operations)

    // /// <summary>
    // /// Suspends the specified entity and raises domain events.
    // /// </summary>
    // Task SuspendEntityAsync<T>(T entity, string reason, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity;

    // /// <summary>
    // /// Reactivates the specified entity and raises domain events.
    // /// </summary>
    // Task ReactivateEntityAsync<T>(T entity, string reason, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity;

    // /// <summary>
    // /// Suspends excessive entities for the specified tenant based on plan limits.
    // /// </summary>
    // Task SuspendExcessiveEntitiesAsync<T>(int tenantId, Func<int, Task<int>> getMaxAllowed, Func<int, Task<List<T>>> getExcessiveEntities, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity;

    // /// <summary>
    // /// Reactivates previously suspended entities for the specified tenant.
    // /// </summary>
    // Task ReactivateSuspendedEntitiesAsync<T>(int tenantId, Func<int, Task<List<T>>> getSuspendedEntities, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity;

    // #endregion

    // #region Subscription State Management

    // /// <summary>
    // /// Handles subscription downgrade for the specified tenant by suspending excessive data.
    // /// </summary>
    // Task HandleDowngradeAsync(int tenantId, Plan newPlan, CancellationToken cancellationToken = default);

    // /// <summary>
    // /// Handles subscription upgrade for the specified tenant by reactivating suspended data.
    // /// </summary>
    // Task HandleUpgradeAsync(int tenantId, Plan newPlan, CancellationToken cancellationToken = default);

    // /// <summary>
    // /// Suspends all excessive data for the specified tenant based on current plan limits.
    // /// </summary>
    // Task SuspendExcessiveDataAsync(int tenantId, CancellationToken cancellationToken = default);

    // /// <summary>
    // /// Reactivates all previously suspended data for the specified tenant.
    // /// </summary>
    // Task ReactivateDataAsync(int tenantId, CancellationToken cancellationToken = default);

    // #endregion
}
