using ConnectFlow.Application.Common.Exceptions;

namespace ConnectFlow.Infrastructure.Services;

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
            return await _context.TenantUserRoles.Include(tur => tur.TenantUser).AnyAsync(tur => tur.TenantUser.UserId == userId && tur.TenantUser.Status == TenantUserStatus.Active && tur.TenantUser.TenantId == tenantId && tur.RoleName == role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role {Role} for current user", role);
            return false;
        }
    }

    public Task<(bool CanAdd, int CurrentCount, int MaxCount)> CanAddEntityAsync(LimitValidationType limitValidationType, CancellationToken cancellationToken = default)
    {
        var tenantId = GetCurrentTenantId();

        return limitValidationType switch
        {
            LimitValidationType.User => CanAddUserAsync(tenantId, cancellationToken),
            LimitValidationType.WhatsAppAccount => CanAddChannelAsync(tenantId, ChannelType.WhatsApp, cancellationToken),
            LimitValidationType.FacebookAccount => CanAddChannelAsync(tenantId, ChannelType.Facebook, cancellationToken),
            LimitValidationType.InstagramAccount => CanAddChannelAsync(tenantId, ChannelType.Instagram, cancellationToken),
            _ => Task.FromResult((false, 0, 0))
        };
    }

    #endregion

    #region Plan Limits Validation

    public async Task<(bool CanAdd, int CurrentCount, int MaxCount)> CanAddUserAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var subscription = await GetActiveSubscriptionAsync(tenantId, cancellationToken);
            if (subscription == null) return (false, 0, 0);

            var currentUserCount = await GetCurrentUserCountAsync(tenantId, cancellationToken);
            return (currentUserCount < subscription.Plan.MaxUsers, currentUserCount, subscription.Plan.MaxUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tenant {TenantId} can add user", tenantId);
            return (false, 0, 0);
        }
    }

    public async Task<(bool CanAdd, int CurrentCount, int MaxCount)> CanAddChannelAsync(int tenantId, ChannelType channelType, CancellationToken cancellationToken = default)
    {
        try
        {
            var subscription = await GetActiveSubscriptionAsync(tenantId, cancellationToken);
            if (subscription == null) return (false, 0, 0);

            var channelsCount = await GetChannelsCountAsync(tenantId, cancellationToken);
            var currentChannelCount = channelsCount.FirstOrDefault(cc => cc.Type == channelType).ChannelCount;
            var currentTotalCount = channelsCount.Sum(cc => cc.ChannelCount);

            return channelType switch
            {
                ChannelType.WhatsApp => (currentChannelCount < subscription.Plan.MaxWhatsAppChannels && currentTotalCount < subscription.Plan.MaxChannels, currentChannelCount, subscription.Plan.MaxWhatsAppChannels),
                ChannelType.Facebook => (currentChannelCount < subscription.Plan.MaxFacebookChannels && currentTotalCount < subscription.Plan.MaxChannels, currentChannelCount, subscription.Plan.MaxFacebookChannels),
                ChannelType.Instagram => (currentChannelCount < subscription.Plan.MaxInstagramChannels && currentTotalCount < subscription.Plan.MaxChannels, currentChannelCount, subscription.Plan.MaxInstagramChannels),
                ChannelType.Telegram => (currentChannelCount < subscription.Plan.MaxTelegramChannels && currentTotalCount < subscription.Plan.MaxChannels, currentChannelCount, subscription.Plan.MaxTelegramChannels),
                _ => (currentTotalCount < subscription.Plan.MaxChannels, currentTotalCount, subscription.Plan.MaxChannels)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tenant {TenantId} can add {ChannelType} channel", tenantId, channelType);
            return (false, 0, 0);
        }
    }

    #endregion

    #region Usage Tracking

    public async Task<int> GetCurrentUserCountAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        //suspended users are filtered out by Query Filters
        return await _context.TenantUsers.Where(tu => tu.TenantId == tenantId && tu.Status == TenantUserStatus.Active).CountAsync(cancellationToken);
    }

    public async Task<List<(ChannelType Type, int ChannelCount)>> GetChannelsCountAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        // Query only once and project counts, Ignore query filters to count all channels for the tenant; manually filter out soft-deleted and suspended channels.
        var query = _context.ChannelAccounts.IgnoreQueryFilters().Where(ca => ca.TenantId == tenantId && !ca.IsDeleted && ca.EntityStatus == EntityStatus.Active);

        var result = await query
            .GroupBy(ca => ca.Type)
            .Select(g => new ValueTuple<ChannelType, int>(g.Key, g.Count()))
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<Dictionary<string, int>> GetUsageStatisticsAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var channelsCount = await GetChannelsCountAsync(tenantId, cancellationToken);
        var userCount = await GetCurrentUserCountAsync(tenantId, cancellationToken);

        var totalChannels = channelsCount.Sum(cc => cc.ChannelCount);
        var whatsAppChannels = channelsCount.FirstOrDefault(cc => cc.Type == ChannelType.WhatsApp).ChannelCount;
        var facebookChannels = channelsCount.FirstOrDefault(cc => cc.Type == ChannelType.Facebook).ChannelCount;
        var instagramChannels = channelsCount.FirstOrDefault(cc => cc.Type == ChannelType.Instagram).ChannelCount;
        var telegramChannels = channelsCount.FirstOrDefault(cc => cc.Type == ChannelType.Telegram).ChannelCount;

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

    // #region Entity Suspension/Reactivation (Generic ISuspendibleEntity Operations)

    // public async Task SuspendEntityAsync<T>(T entity, string reason, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity
    // {
    //     try
    //     {
    //         entity.EntityStatus = EntityStatus.Suspended;
    //         entity.SuspendedAt = DateTimeOffset.UtcNow;
    //         entity.ResumedAt = null;

    //         // Add domain event
    //         entity.AddDomainEvent(new EntitySuspendedEvent<T>(entity, reason));

    //         await _context.SaveChangesAsync(cancellationToken);

    //         _logger.LogInformation("Suspended {EntityType} entity {EntityId} for reason: {Reason}", typeof(T).Name, entity.Id, reason);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to suspend {EntityType} entity {EntityId}", typeof(T).Name, entity.Id);
    //         throw;
    //     }
    // }

    // public async Task ReactivateEntityAsync<T>(T entity, string reason, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity
    // {
    //     try
    //     {
    //         entity.EntityStatus = EntityStatus.Active;
    //         entity.ResumedAt = DateTimeOffset.UtcNow;
    //         entity.SuspendedAt = null;

    //         // Add domain event
    //         entity.AddDomainEvent(new EntityReactivatedEvent<T>(entity, reason));

    //         await _context.SaveChangesAsync(cancellationToken);

    //         _logger.LogInformation("Reactivated {EntityType} entity {EntityId} for reason: {Reason}", typeof(T).Name, entity.Id, reason);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to reactivate {EntityType} entity {EntityId}", typeof(T).Name, entity.Id);
    //         throw;
    //     }
    // }

    // public async Task SuspendExcessiveEntitiesAsync<T>(int tenantId, Func<int, Task<int>> getMaxAllowed, Func<int, Task<List<T>>> getExcessiveEntities, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity
    // {
    //     try
    //     {
    //         var maxAllowed = await getMaxAllowed(tenantId);
    //         var excessiveEntities = await getExcessiveEntities(tenantId);

    //         foreach (var entity in excessiveEntities)
    //         {
    //             await SuspendEntityAsync(entity, $"Exceeded plan limit of {maxAllowed}", cancellationToken);
    //         }

    //         _logger.LogInformation("Suspended {Count} {EntityType} entities for tenant {TenantId} due to plan limits",
    //             excessiveEntities.Count, typeof(T).Name, tenantId);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to suspend excessive {EntityType} entities for tenant {TenantId}", typeof(T).Name, tenantId);
    //         throw;
    //     }
    // }

    // public async Task ReactivateSuspendedEntitiesAsync<T>(int tenantId, Func<int, Task<List<T>>> getSuspendedEntities, CancellationToken cancellationToken = default) where T : BaseAuditableEntity, ISuspendibleEntity
    // {
    //     try
    //     {
    //         var suspendedEntities = await getSuspendedEntities(tenantId);

    //         foreach (var entity in suspendedEntities)
    //         {
    //             await ReactivateEntityAsync(entity, "Plan upgrade - reactivating suspended entities", cancellationToken);
    //         }

    //         _logger.LogInformation("Reactivated {Count} {EntityType} entities for tenant {TenantId}",
    //             suspendedEntities.Count, typeof(T).Name, tenantId);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to reactivate {EntityType} entities for tenant {TenantId}", typeof(T).Name, tenantId);
    //         throw;
    //     }
    // }

    // #endregion

    // #region Subscription State Management

    // public async Task HandleDowngradeAsync(int tenantId, Plan newPlan, CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         // Check current usage against new plan limits
    //         var currentUsage = await GetUsageStatisticsAsync(tenantId, cancellationToken);

    //         // Suspend excess users if needed
    //         if (currentUsage["Users"] > newPlan.MaxUsers)
    //         {
    //             await SuspendExcessiveEntitiesAsync<TenantUser>(
    //                 tenantId,
    //                 _ => Task.FromResult(newPlan.MaxUsers),
    //                 async tenantId => await _context.TenantUsers
    //                     .Where(tu => tu.TenantId == tenantId && tu.Status == TenantUserStatus.Active)
    //                     .OrderByDescending(tu => tu.JoinedAt)
    //                     .Skip(newPlan.MaxUsers)
    //                     .ToListAsync(cancellationToken),
    //                 cancellationToken
    //             );
    //         }

    //         // Suspend excess channels
    //         await SuspendExcessiveChannelsAsync(tenantId, ChannelType.WhatsApp, newPlan.MaxWhatsAppChannels, cancellationToken);
    //         await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Facebook, newPlan.MaxFacebookChannels, cancellationToken);
    //         await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Instagram, newPlan.MaxInstagramChannels, cancellationToken);
    //         await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Telegram, newPlan.MaxTelegramChannels, cancellationToken);

    //         _logger.LogInformation("Handled downgrade for tenant {TenantId} to plan {PlanId}", tenantId, newPlan.Id);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to handle downgrade for tenant {TenantId}", tenantId);
    //         throw;
    //     }
    // }

    // public async Task HandleUpgradeAsync(int tenantId, Plan newPlan, CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         // Reactivate any suspended data that now falls within limits
    //         await ReactivateDataAsync(tenantId, cancellationToken);

    //         _logger.LogInformation("Handled upgrade for tenant {TenantId} to plan {PlanId}", tenantId, newPlan.Id);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to handle upgrade for tenant {TenantId}", tenantId);
    //         throw;
    //     }
    // }

    // public async Task SuspendExcessiveDataAsync(int tenantId, CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         var subscription = await GetActiveSubscriptionAsync(tenantId, cancellationToken);
    //         if (subscription == null)
    //         {
    //             _logger.LogWarning("No active subscription found for tenant {TenantId}, cannot suspend data", tenantId);
    //             return;
    //         }

    //         var plan = subscription.Plan;

    //         // Suspend excessive users
    //         var userCount = await GetCurrentUserCountAsync(tenantId, cancellationToken);
    //         if (userCount > plan.MaxUsers)
    //         {
    //             await SuspendExcessiveEntitiesAsync<TenantUser>(
    //                 tenantId,
    //                 _ => Task.FromResult(plan.MaxUsers),
    //                 async tenantId => await _context.TenantUsers
    //                     .Where(tu => tu.TenantId == tenantId && tu.Status == TenantUserStatus.Active)
    //                     .OrderByDescending(tu => tu.JoinedAt)
    //                     .Skip(plan.MaxUsers)
    //                     .ToListAsync(cancellationToken),
    //                 cancellationToken
    //             );
    //         }

    //         // Suspend excessive channels
    //         await SuspendExcessiveChannelsAsync(tenantId, ChannelType.WhatsApp, plan.MaxWhatsAppChannels, cancellationToken);
    //         await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Facebook, plan.MaxFacebookChannels, cancellationToken);
    //         await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Instagram, plan.MaxInstagramChannels, cancellationToken);
    //         await SuspendExcessiveChannelsAsync(tenantId, ChannelType.Telegram, plan.MaxTelegramChannels, cancellationToken);

    //         _logger.LogInformation("Suspended excessive data for tenant {TenantId}", tenantId);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to suspend excessive data for tenant {TenantId}", tenantId);
    //         throw;
    //     }
    // }

    // public async Task ReactivateDataAsync(int tenantId, CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         // Reactivate suspended users
    //         await ReactivateSuspendedEntitiesAsync<TenantUser>(
    //             tenantId,
    //             async tenantId => await _context.TenantUsers
    //                 .Where(tu => tu.TenantId == tenantId && tu.EntityStatus == EntityStatus.Suspended)
    //                 .ToListAsync(cancellationToken),
    //             cancellationToken
    //         );

    //         // Reactivate suspended channels
    //         await ReactivateSuspendedEntitiesAsync<ChannelAccount>(
    //             tenantId,
    //             async tenantId => await _context.ChannelAccounts
    //                 .Where(ca => ca.TenantId == tenantId && ca.EntityStatus == EntityStatus.Suspended)
    //                 .ToListAsync(cancellationToken),
    //             cancellationToken
    //         );

    //         _logger.LogInformation("Reactivated suspended data for tenant {TenantId}", tenantId);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to reactivate data for tenant {TenantId}", tenantId);
    //         throw;
    //     }
    // }

    // #endregion

    #region Private Helper Methods

    private int GetCurrentTenantId()
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            throw new TenantNotFoundException();
        }
        return tenantId.Value;
    }

    private int GetCurrentApplicationUserId()
    {
        var userId = _contextManager.GetCurrentApplicationUserId();
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException();
        }
        return userId.Value;
    }

    private bool IsSuperAdminAllowed(bool allowSuperAdmin)
    {
        return allowSuperAdmin && _contextManager.IsSuperAdmin();
    }

    // private async Task SuspendExcessiveChannelsAsync(int tenantId, ChannelType channelType, int maxAllowed, CancellationToken cancellationToken)
    // {
    //     var channelsCount = await GetChannelsCountAsync(tenantId, cancellationToken);

    //     var currentCount = channelsCount.FirstOrDefault(cc => cc.Type == channelType);

    //     if (currentCount.ChannelCount > maxAllowed)
    //     {
    //         await SuspendExcessiveEntitiesAsync<ChannelAccount>(
    //             tenantId,
    //             _ => Task.FromResult(maxAllowed),
    //             async tenantId => await _context.ChannelAccounts
    //                 .Where(ca => ca.TenantId == tenantId && ca.Type == channelType && !ca.IsDeleted && ca.EntityStatus == EntityStatus.Active)
    //                 .OrderByDescending(ca => ca.Created)
    //                 .Skip(maxAllowed)
    //                 .ToListAsync(cancellationToken),
    //             cancellationToken
    //         );

    //         _logger.LogInformation("Processed suspension check for {ChannelType} channels in tenant {TenantId}", channelType, tenantId);
    //     }
    // }

    #endregion
}
