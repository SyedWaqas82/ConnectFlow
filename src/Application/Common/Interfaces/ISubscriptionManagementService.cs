namespace ConnectFlow.Application.Common.Interfaces;

/// <summary>
/// Provides methods for managing subscriptions, enforcing limits, handling entity suspension/reactivation, and orchestrating domain events.
/// </summary>
public interface ISubscriptionManagementService
{
    #region Subscription Management

    /// <summary>
    /// Gets the active subscription for a tenant.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The active subscription, or null if none exists.</returns>
    Task<Subscription?> GetActiveSubscriptionAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tenant has an active subscription.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the tenant has an active subscription; otherwise, false.</returns>
    Task<bool> HasActiveSubscriptionAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the subscription history for a tenant.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of subscriptions.</returns>
    Task<List<Subscription>> GetSubscriptionHistoryAsync(int tenantId, CancellationToken cancellationToken = default);

    #endregion

    #region Current Context Operations

    /// <summary>
    /// Checks if the current user's tenant has an active subscription.
    /// </summary>
    /// <param name="allowSuperAdmin">Allow super admin to bypass check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if active; otherwise, false.</returns>
    Task<bool> IsCurrentUserFromCurrentTenantHasActiveSubscriptionAsync(bool allowSuperAdmin = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the current user has a specific role in the current tenant.
    /// </summary>
    /// <param name="role">Role name.</param>
    /// <param name="allowSuperAdmin">Allow super admin to bypass check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user has the role; otherwise, false.</returns>
    Task<bool> IsCurrentUserFromCurrentTenantHasRoleAsync(string role, bool allowSuperAdmin = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the current tenant can add an entity type based on plan limits.
    /// </summary>
    /// <param name="limitValidationType">Type of limit validation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple indicating if addition is allowed, current count, and max count.</returns>
    Task<(bool CanAdd, int CurrentCount, int MaxCount)> CanAddEntityAsync(LimitValidationType limitValidationType, CancellationToken cancellationToken = default);

    #endregion

    #region Plan Limits Validation

    /// <summary>
    /// Checks if a tenant can add a user based on plan limits.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple indicating if addition is allowed, current count, and max count.</returns>
    Task<(bool CanAdd, int CurrentCount, int MaxCount)> CanAddUserAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tenant can add a channel of a specific type based on plan limits.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="channelType">Channel type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple indicating if addition is allowed, current count, and max count.</returns>
    Task<(bool CanAdd, int CurrentCount, int MaxCount)> CanAddChannelAsync(int tenantId, ChannelType channelType, CancellationToken cancellationToken = default);

    #endregion

    #region Usage Tracking

    /// <summary>
    /// Gets the current user count for a tenant.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User count.</returns>
    Task<int> GetCurrentUserCountAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the channel counts for a tenant.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of channel types and their counts.</returns>
    Task<List<(ChannelType Type, int ChannelCount)>> GetChannelsCountAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets usage statistics for a tenant.
    /// </summary>
    /// <param name="tenantId">Tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary of usage statistics.</returns>
    Task<Dictionary<string, int>> GetUsageStatisticsAsync(int tenantId, CancellationToken cancellationToken = default);

    #endregion

    #region Entity Suspension/Reactivation (Generic ISuspendibleEntity Operations)

    /// <summary>
    /// Synchronizes entities with subscription limits after status changes.
    /// Suspends or restores entities based on plan limits.
    /// </summary>
    Task SyncEntitiesWithLimitsAsync(int tenantId, CancellationToken cancellationToken = default);

    #endregion
}