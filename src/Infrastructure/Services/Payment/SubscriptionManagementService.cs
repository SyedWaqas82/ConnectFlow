using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Events.Mediator.Entities;

namespace ConnectFlow.Infrastructure.Services.Payment;

public class SubscriptionManagementService : ISubscriptionManagementService
{
    private readonly IApplicationDbContext _context;
    private readonly IContextManager _contextManager;
    private readonly ILogger<SubscriptionManagementService> _logger;

    public SubscriptionManagementService(IApplicationDbContext context, IContextManager contextManager, ILogger<SubscriptionManagementService> logger)
    {
        _context = context;
        _contextManager = contextManager;
        _logger = logger;
    }

    #region Subscription Management

    public async Task<Subscription?> GetActiveSubscriptionAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        // Subscription does not have global query filters
        return await _context.Subscriptions.Include(s => s.Plan).Where(s => s.TenantId == tenantId && (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trialing || s.Status == SubscriptionStatus.PastDue)).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HasActiveSubscriptionAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        // Subscription does not have global query filters
        return await _context.Subscriptions.AnyAsync(s => s.TenantId == tenantId && (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trialing || s.Status == SubscriptionStatus.PastDue), cancellationToken);
    }

    public async Task<List<Subscription>> GetSubscriptionHistoryAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        // Subscription does not have global query filters
        return await _context.Subscriptions.Include(s => s.Plan).Where(s => s.TenantId == tenantId).OrderByDescending(s => s.Created).ToListAsync(cancellationToken);
    }

    #endregion

    #region Current Context Operations

    public async Task<bool> IsCurrentUserFromCurrentTenantHasActiveSubscriptionAsync(bool allowSuperAdmin = true, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = GetCurrentTenantId();
            var userId = GetCurrentApplicationUserId();

            if (IsSuperAdminAllowed(allowSuperAdmin))
                return true;

            // Tenant User suspension check is made at query filter level
            var isTenantUser = await _context.TenantUsers.AnyAsync(tu => tu.TenantId == tenantId && tu.UserId == userId && tu.Status == TenantUserStatus.Active);

            if (!isTenantUser)
                return false;

            return await HasActiveSubscriptionAsync(tenantId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking active subscription for current user/tenant");
            return false;
        }
    }

    public async Task<bool> IsCurrentUserFromCurrentTenantHasRoleAsync(string role, bool allowSuperAdmin = true, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = GetCurrentTenantId();
            var userId = GetCurrentApplicationUserId();

            if (IsSuperAdminAllowed(allowSuperAdmin))
                return true;

            // Tenant User suspension check is made at query filter level
            return await _context.TenantUserRoles.Include(tur => tur.TenantUser).AnyAsync(tur => tur.TenantUser.UserId == userId && tur.TenantUser.Status == TenantUserStatus.Active &&
                tur.TenantUser.TenantId == tenantId && tur.RoleName == role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role {Role} for current user", role);
            return false;
        }
    }

    #endregion

    // #region Plan Limits Validation

    // public async Task<bool> CanAddUserAsync(int tenantId, CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         var subscription = await GetActiveSubscriptionAsync(tenantId, cancellationToken);
    //         if (subscription == null) return false;

    //         var currentUserCount = await GetCurrentUserCountAsync(tenantId, cancellationToken);
    //         return currentUserCount < subscription.Plan.MaxUsers;
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error checking if tenant {TenantId} can add user", tenantId);
    //         return false;
    //     }
    // }

    // public async Task<bool> CanCurrentTenantAddUserAsync(CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         var tenantId = GetCurrentTenantId();
    //         return await CanAddUserAsync(tenantId, cancellationToken);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error checking if current tenant can add user");
    //         return false;
    //     }
    // }

    // public async Task<bool> CanAddChannelAsync(int tenantId, ChannelType channelType, CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         var subscription = await GetActiveSubscriptionAsync(tenantId, cancellationToken);
    //         if (subscription == null) return false;

    //         var currentChannelCount = await GetCurrentChannelCountAsync(tenantId, channelType, cancellationToken);

    //         return channelType switch
    //         {
    //             ChannelType.WhatsApp => currentChannelCount < subscription.Plan.MaxWhatsAppChannels,
    //             ChannelType.Facebook => currentChannelCount < subscription.Plan.MaxFacebookChannels,
    //             ChannelType.Instagram => currentChannelCount < subscription.Plan.MaxInstagramChannels,
    //             ChannelType.Telegram => currentChannelCount < subscription.Plan.MaxTelegramChannels,
    //             _ => currentChannelCount < subscription.Plan.MaxChannels
    //         };
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error checking if tenant {TenantId} can add {ChannelType} channel", tenantId, channelType);
    //         return false;
    //     }
    // }

    // public async Task<bool> CanCurrentTenantAddChannelAsync(ChannelType channelType, CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         var tenantId = GetCurrentTenantId();
    //         return await CanAddChannelAsync(tenantId, channelType, cancellationToken);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error checking if current tenant can add {ChannelType} channel", channelType);
    //         return false;
    //     }
    // }

    // public async Task<bool> CanPerformOperationAsync(int tenantId, string operationType, CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         var hasActiveSubscription = await HasActiveSubscriptionAsync(tenantId, cancellationToken);
    //         if (!hasActiveSubscription)
    //         {
    //             return false;
    //         }

    //         // Add specific operation type validations here
    //         // For example: AI token limits, API call limits, etc.
    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error checking if tenant {TenantId} can perform operation {OperationType}", tenantId, operationType);
    //         return false;
    //     }
    // }

    // public async Task<bool> CanCurrentTenantPerformOperationAsync(string operationType, CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         var tenantId = GetCurrentTenantId();
    //         return await CanPerformOperationAsync(tenantId, operationType, cancellationToken);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error checking if current tenant can perform operation {OperationType}", operationType);
    //         return false;
    //     }
    // }

    // #endregion

    #region Usage Tracking

    public async Task<int> GetCurrentUserCountAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        //suspended users are filtered out by Query Filters
        return await _context.TenantUsers.Where(tu => tu.TenantId == tenantId && tu.Status == TenantUserStatus.Active).CountAsync(cancellationToken);
    }

    public async Task<int> GetCurrentChannelCountAsync(int tenantId, ChannelType? channelType = null, CancellationToken cancellationToken = default)
    {
        // Ignore query filters to count all channels for the tenant; manually filter out soft-deleted and suspended channels.
        var query = _context.ChannelAccounts.IgnoreQueryFilters().Where(ca => ca.TenantId == tenantId && !ca.IsDeleted && ca.EntityStatus == EntityStatus.Active);

        if (channelType.HasValue)
        {
            query = query.Where(ca => ca.Type == channelType.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetUsageStatisticsAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var userCount = await GetCurrentUserCountAsync(tenantId, cancellationToken);
        var totalChannels = await GetCurrentChannelCountAsync(tenantId, null, cancellationToken);
        var whatsAppChannels = await GetCurrentChannelCountAsync(tenantId, ChannelType.WhatsApp, cancellationToken);
        var facebookChannels = await GetCurrentChannelCountAsync(tenantId, ChannelType.Facebook, cancellationToken);
        var instagramChannels = await GetCurrentChannelCountAsync(tenantId, ChannelType.Instagram, cancellationToken);
        var telegramChannels = await GetCurrentChannelCountAsync(tenantId, ChannelType.Telegram, cancellationToken);

        return new Dictionary<string, int>
        {
            { "Users", userCount },
            { "TotalChannels", totalChannels },
            { "WhatsAppChannels", whatsAppChannels },
            { "FacebookChannels", facebookChannels },
            { "InstagramChannels", instagramChannels },
            { "TelegramChannels", telegramChannels }
        };
    }

    #endregion

    // #region Validation with Exception Throwing

    // public async Task ValidateActiveSubscriptionAsync(int tenantId, CancellationToken cancellationToken = default)
    // {
    //     var hasActiveSubscription = await HasActiveSubscriptionAsync(tenantId, cancellationToken);
    //     if (!hasActiveSubscription)
    //     {
    //         throw new SubscriptionRequiredException($"Tenant {tenantId} does not have an active subscription");
    //     }
    // }

    // public async Task ValidateUserLimitAsync(int tenantId, CancellationToken cancellationToken = default)
    // {
    //     var canAddUser = await CanAddUserAsync(tenantId, cancellationToken);
    //     if (!canAddUser)
    //     {
    //         var subscription = await GetActiveSubscriptionAsync(tenantId, cancellationToken);
    //         var currentUserCount = await GetCurrentUserCountAsync(tenantId, cancellationToken);

    //         throw new SubscriptionLimitExceededException("Users", subscription!.Plan.MaxUsers, currentUserCount);
    //     }
    // }

    // public async Task ValidateChannelLimitAsync(int tenantId, ChannelType channelType, CancellationToken cancellationToken = default)
    // {
    //     var canAddChannel = await CanAddChannelAsync(tenantId, channelType, cancellationToken);
    //     if (!canAddChannel)
    //     {
    //         var subscription = await GetActiveSubscriptionAsync(tenantId, cancellationToken);
    //         var currentChannelCount = await GetCurrentChannelCountAsync(tenantId, channelType, cancellationToken);

    //         var maxChannels = channelType switch
    //         {
    //             ChannelType.WhatsApp => subscription!.Plan.MaxWhatsAppChannels,
    //             ChannelType.Facebook => subscription!.Plan.MaxFacebookChannels,
    //             ChannelType.Instagram => subscription!.Plan.MaxInstagramChannels,
    //             ChannelType.Telegram => subscription!.Plan.MaxTelegramChannels,
    //             _ => subscription!.Plan.MaxChannels
    //         };

    //         throw new SubscriptionLimitExceededException($"{channelType} Channels", maxChannels, currentChannelCount);
    //     }
    // }

    // public async Task ValidateOperationLimitAsync(int tenantId, string operationType, CancellationToken cancellationToken = default)
    // {
    //     var canPerformOperation = await CanPerformOperationAsync(tenantId, operationType, cancellationToken);
    //     if (!canPerformOperation)
    //     {
    //         throw new SubscriptionLimitExceededException(operationType, 0, 1); // Generic limit exceeded
    //     }
    // }

    // #endregion

    #region Entity Suspension/Reactivation (Generic ISuspendibleEntity Operations)

    public async Task SuspendEntityAsync<T>(T entity, string reason, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity
    {
        try
        {
            entity.EntityStatus = EntityStatus.Suspended;
            entity.SuspendedAt = DateTimeOffset.UtcNow;
            entity.ResumedAt = null;

            // Add domain event
            entity.AddDomainEvent(new EntitySuspendedEvent<T>(entity, reason));

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Suspended {EntityType} entity {EntityId} for reason: {Reason}", typeof(T).Name, entity.Id, reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to suspend {EntityType} entity {EntityId}", typeof(T).Name, entity.Id);
            throw;
        }
    }

    public async Task ReactivateEntityAsync<T>(T entity, string reason, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity
    {
        try
        {
            entity.EntityStatus = EntityStatus.Active;
            entity.ResumedAt = DateTimeOffset.UtcNow;
            entity.SuspendedAt = null;

            // Add domain event
            entity.AddDomainEvent(new EntityReactivatedEvent<T>(entity, reason));

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reactivated {EntityType} entity {EntityId} for reason: {Reason}", typeof(T).Name, entity.Id, reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reactivate {EntityType} entity {EntityId}", typeof(T).Name, entity.Id);
            throw;
        }
    }

    public async Task SuspendExcessiveEntitiesAsync<T>(int tenantId, Func<int, Task<int>> getMaxAllowed, Func<int, Task<List<T>>> getExcessiveEntities, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity
    {
        try
        {
            var maxAllowed = await getMaxAllowed(tenantId);
            var excessiveEntities = await getExcessiveEntities(tenantId);

            foreach (var entity in excessiveEntities)
            {
                await SuspendEntityAsync(entity, $"Exceeded plan limit of {maxAllowed}", cancellationToken);
            }

            _logger.LogInformation("Suspended {Count} {EntityType} entities for tenant {TenantId} due to plan limits",
                excessiveEntities.Count, typeof(T).Name, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to suspend excessive {EntityType} entities for tenant {TenantId}", typeof(T).Name, tenantId);
            throw;
        }
    }

    public async Task ReactivateSuspendedEntitiesAsync<T>(int tenantId, Func<int, Task<List<T>>> getSuspendedEntities, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity
    {
        try
        {
            var suspendedEntities = await getSuspendedEntities(tenantId);

            foreach (var entity in suspendedEntities)
            {
                await ReactivateEntityAsync(entity, "Plan upgrade - reactivating suspended entities", cancellationToken);
            }

            _logger.LogInformation("Reactivated {Count} {EntityType} entities for tenant {TenantId}",
                suspendedEntities.Count, typeof(T).Name, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reactivate {EntityType} entities for tenant {TenantId}", typeof(T).Name, tenantId);
            throw;
        }
    }

    #endregion

    #region Subscription State Management

    public async Task HandleDowngradeAsync(int tenantId, Plan newPlan, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check current usage against new plan limits
            var currentUsage = await GetUsageStatisticsAsync(tenantId, cancellationToken);

            // Suspend excess users if needed
            if (currentUsage["Users"] > newPlan.MaxUsers)
            {
                await SuspendExcessiveEntitiesAsync<TenantUser>(
                    tenantId,
                    _ => Task.FromResult(newPlan.MaxUsers),
                    async tenantId => await _context.TenantUsers
                        .Where(tu => tu.TenantId == tenantId && tu.Status == TenantUserStatus.Active)
                        .OrderByDescending(tu => tu.JoinedAt)
                        .Skip(newPlan.MaxUsers)
                        .ToListAsync(cancellationToken),
                    cancellationToken
                );
            }

            // Suspend excess channels
            await SuspendExcessiveChannelsAsync(tenantId, ChannelType.WhatsApp, newPlan.MaxWhatsAppChannels, cancellationToken);
            await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Facebook, newPlan.MaxFacebookChannels, cancellationToken);
            await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Instagram, newPlan.MaxInstagramChannels, cancellationToken);
            await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Telegram, newPlan.MaxTelegramChannels, cancellationToken);

            _logger.LogInformation("Handled downgrade for tenant {TenantId} to plan {PlanId}", tenantId, newPlan.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle downgrade for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task HandleUpgradeAsync(int tenantId, Plan newPlan, CancellationToken cancellationToken = default)
    {
        try
        {
            // Reactivate any suspended data that now falls within limits
            await ReactivateDataAsync(tenantId, cancellationToken);

            _logger.LogInformation("Handled upgrade for tenant {TenantId} to plan {PlanId}", tenantId, newPlan.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle upgrade for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task SuspendExcessiveDataAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var subscription = await GetActiveSubscriptionAsync(tenantId, cancellationToken);
            if (subscription == null)
            {
                _logger.LogWarning("No active subscription found for tenant {TenantId}, cannot suspend data", tenantId);
                return;
            }

            var plan = subscription.Plan;

            // Suspend excessive users
            var userCount = await GetCurrentUserCountAsync(tenantId, cancellationToken);
            if (userCount > plan.MaxUsers)
            {
                await SuspendExcessiveEntitiesAsync<TenantUser>(
                    tenantId,
                    _ => Task.FromResult(plan.MaxUsers),
                    async tenantId => await _context.TenantUsers
                        .Where(tu => tu.TenantId == tenantId && tu.Status == TenantUserStatus.Active)
                        .OrderByDescending(tu => tu.JoinedAt)
                        .Skip(plan.MaxUsers)
                        .ToListAsync(cancellationToken),
                    cancellationToken
                );
            }

            // Suspend excessive channels
            await SuspendExcessiveChannelsAsync(tenantId, ChannelType.WhatsApp, plan.MaxWhatsAppChannels, cancellationToken);
            await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Facebook, plan.MaxFacebookChannels, cancellationToken);
            await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Instagram, plan.MaxInstagramChannels, cancellationToken);
            await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Telegram, plan.MaxTelegramChannels, cancellationToken);

            _logger.LogInformation("Suspended excessive data for tenant {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to suspend excessive data for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task ReactivateDataAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Reactivate suspended users
            await ReactivateSuspendedEntitiesAsync<TenantUser>(
                tenantId,
                async tenantId => await _context.TenantUsers
                    .Where(tu => tu.TenantId == tenantId && tu.EntityStatus == EntityStatus.Suspended)
                    .ToListAsync(cancellationToken),
                cancellationToken
            );

            // Reactivate suspended channels
            await ReactivateSuspendedEntitiesAsync<ChannelAccount>(
                tenantId,
                async tenantId => await _context.ChannelAccounts
                    .Where(ca => ca.TenantId == tenantId && ca.EntityStatus == EntityStatus.Suspended)
                    .ToListAsync(cancellationToken),
                cancellationToken
            );

            _logger.LogInformation("Reactivated suspended data for tenant {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reactivate data for tenant {TenantId}", tenantId);
            throw;
        }
    }

    #endregion

    #region Private Helper Methods

    private int GetCurrentTenantId()
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            throw new ForbiddenAccessException();
        }
        return tenantId.Value;
    }

    private Guid GetCurrentUserId()
    {
        var userId = _contextManager.GetCurrentUserId();
        if (!userId.HasValue)
        {
            throw new ForbiddenAccessException();
        }
        return userId.Value;
    }

    private int GetCurrentApplicationUserId()
    {
        var userId = _contextManager.GetCurrentApplicationUserId();
        if (!userId.HasValue)
        {
            throw new ForbiddenAccessException();
        }
        return userId.Value;
    }

    private bool IsSuperAdminAllowed(bool allowSuperAdmin)
    {
        return allowSuperAdmin && _contextManager.IsSuperAdmin();
    }

    private async Task SuspendExcessiveChannelsAsync(int tenantId, ChannelType channelType, int maxAllowed, CancellationToken cancellationToken)
    {
        var currentCount = await GetCurrentChannelCountAsync(tenantId, channelType, cancellationToken);

        if (currentCount > maxAllowed)
        {
            await SuspendExcessiveEntitiesAsync<ChannelAccount>(
                tenantId,
                _ => Task.FromResult(maxAllowed),
                async tenantId => await _context.ChannelAccounts
                    .Where(ca => ca.TenantId == tenantId && ca.Type == channelType &&
                               !ca.IsDeleted && ca.EntityStatus == EntityStatus.Active)
                    .OrderByDescending(ca => ca.Created)
                    .Skip(maxAllowed)
                    .ToListAsync(cancellationToken),
                cancellationToken
            );

            _logger.LogInformation("Processed suspension check for {ChannelType} channels in tenant {TenantId}", channelType, tenantId);
        }
    }

    #endregion
}
