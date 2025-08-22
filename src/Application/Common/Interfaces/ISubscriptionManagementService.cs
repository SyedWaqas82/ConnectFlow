using ConnectFlow.Domain.Entities;
using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Application.Common.Interfaces;

/// <summary>
/// Comprehensive subscription management service that handles subscription validation, 
/// limits enforcement, entity suspension/reactivation, and domain event orchestration
/// </summary>
public interface ISubscriptionManagementService
{
    #region Subscription Management

    /// <summary>
    /// Gets the active subscription for a tenant
    /// </summary>
    Task<Subscription?> GetActiveSubscriptionAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tenant has an active subscription
    /// </summary>
    Task<bool> HasActiveSubscriptionAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subscription history for a tenant
    /// </summary>
    Task<List<Subscription>> GetSubscriptionHistoryAsync(int tenantId, CancellationToken cancellationToken = default);

    #endregion

    #region Current Context Operations

    /// <summary>
    /// Checks if current user's tenant has an active subscription
    /// </summary>
    Task<bool> IsCurrentUserFromCurrentTenantHasActiveSubscriptionAsync(bool allowSuperAdmin = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if current user has a specific role in current tenant
    /// </summary>
    Task<bool> IsCurrentUserFromCurrentTenantHasRoleAsync(string role, bool allowSuperAdmin = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current tenant ID from context
    /// </summary>
    Task<int> GetCurrentTenantIdAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current user ID from context
    /// </summary>
    Task<int> GetCurrentUserIdAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Plan Limits Validation

    /// <summary>
    /// Checks if a tenant can add a user based on plan limits
    /// </summary>
    Task<bool> CanAddUserAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if current tenant can add a user
    /// </summary>
    Task<bool> CanCurrentTenantAddUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tenant can add a channel based on plan limits
    /// </summary>
    Task<bool> CanAddChannelAsync(int tenantId, ChannelType channelType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if current tenant can add a channel
    /// </summary>
    Task<bool> CanCurrentTenantAddChannelAsync(ChannelType channelType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tenant can perform a specific operation
    /// </summary>
    Task<bool> CanPerformOperationAsync(int tenantId, string operationType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if current tenant can perform a specific operation
    /// </summary>
    Task<bool> CanCurrentTenantPerformOperationAsync(string operationType, CancellationToken cancellationToken = default);

    #endregion

    #region Usage Tracking

    /// <summary>
    /// Gets current user count for a tenant
    /// </summary>
    Task<int> GetCurrentUserCountAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current channel count for a tenant
    /// </summary>
    Task<int> GetCurrentChannelCountAsync(int tenantId, ChannelType? channelType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets comprehensive usage statistics for a tenant
    /// </summary>
    Task<Dictionary<string, int>> GetUsageStatisticsAsync(int tenantId, CancellationToken cancellationToken = default);

    #endregion

    #region Validation with Exception Throwing

    /// <summary>
    /// Validates that a tenant has an active subscription, throws exception if not
    /// </summary>
    Task ValidateActiveSubscriptionAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates user limit for a tenant, throws exception if exceeded
    /// </summary>
    Task ValidateUserLimitAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates channel limit for a tenant, throws exception if exceeded
    /// </summary>
    Task ValidateChannelLimitAsync(int tenantId, ChannelType channelType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates operation limit for a tenant, throws exception if exceeded
    /// </summary>
    Task ValidateOperationLimitAsync(int tenantId, string operationType, CancellationToken cancellationToken = default);

    #endregion

    #region Entity Suspension/Reactivation (Generic ISuspendibleEntity Operations)

    /// <summary>
    /// Suspends a suspendible entity and raises domain events
    /// </summary>
    Task SuspendEntityAsync<T>(T entity, string reason, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity;

    /// <summary>
    /// Reactivates a suspendible entity and raises domain events
    /// </summary>
    Task ReactivateEntityAsync<T>(T entity, string reason, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity;

    /// <summary>
    /// Bulk suspend entities based on tenant plan limits
    /// </summary>
    Task SuspendExcessiveEntitiesAsync<T>(int tenantId, Func<int, Task<int>> getMaxAllowed, Func<int, Task<List<T>>> getExcessiveEntities, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity;

    /// <summary>
    /// Bulk reactivate previously suspended entities
    /// </summary>
    Task ReactivateSuspendedEntitiesAsync<T>(int tenantId, Func<int, Task<List<T>>> getSuspendedEntities, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity;

    #endregion

    #region Subscription State Management

    /// <summary>
    /// Handles subscription downgrade by suspending excessive data
    /// </summary>
    Task HandleDowngradeAsync(int tenantId, Plan newPlan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles subscription upgrade by reactivating suspended data
    /// </summary>
    Task HandleUpgradeAsync(int tenantId, Plan newPlan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Suspends all excessive data for a tenant based on current plan limits
    /// </summary>
    Task SuspendExcessiveDataAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reactivates all previously suspended data for a tenant
    /// </summary>
    Task ReactivateDataAsync(int tenantId, CancellationToken cancellationToken = default);

    #endregion
}
