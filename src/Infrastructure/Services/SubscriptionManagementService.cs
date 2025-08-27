using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Domain.Events.Mediator.ChannelAccounts;
using ConnectFlow.Domain.Events.Mediator.TenantUsers;

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
            var applicationUserId = GetCurrentApplicationUserId();

            if (IsSuperAdminAllowed(allowSuperAdmin))
                return true;

            // Tenant User suspension check is made at query filter level
            var isTenantUser = await _context.TenantUsers.AnyAsync(tu => tu.TenantId == tenantId && tu.ApplicationUserId == applicationUserId && tu.Status == TenantUserStatus.Active);

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
            var applicationUserId = GetCurrentApplicationUserId();

            if (IsSuperAdminAllowed(allowSuperAdmin))
                return true;

            // Tenant User suspension check is made at query filter level
            return await _context.TenantUserRoles.Include(tur => tur.TenantUser).AnyAsync(tur => tur.TenantUser.ApplicationUserId == applicationUserId && tur.TenantUser.Status == TenantUserStatus.Active && tur.TenantUser.TenantId == tenantId && tur.RoleName == role);
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

    #region Entity Suspension/Reactivation Operations

    public async Task SyncEntitiesWithLimitsAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting comprehensive entity sync with plan limits for tenant {TenantId}", tenantId);

        try
        {
            // Get the current active subscription and plan
            var subscription = await GetActiveSubscriptionAsync(tenantId, cancellationToken);

            if (subscription?.Plan == null)
            {
                _logger.LogWarning("No active subscription found for tenant {TenantId}. Suspending all entities.", tenantId);
                await SuspendAllEntitiesAsync(tenantId, "No active subscription", cancellationToken);
                return;
            }

            var plan = subscription.Plan;
            _logger.LogInformation("Syncing entities for tenant {TenantId} with plan {PlanName} (MaxUsers: {MaxUsers}, MaxChannels: {MaxChannels})", tenantId, plan.Name, plan.MaxUsers, plan.MaxChannels);

            // Sync users with plan limits
            await SyncTenantUsersWithPlanLimitsAsync(tenantId, plan.MaxUsers, cancellationToken);

            // PHASE 1: Sync each channel type to their individual limits first
            await SyncChannelsAsync(tenantId, ChannelType.WhatsApp, plan.MaxWhatsAppChannels, cancellationToken);
            await SyncChannelsAsync(tenantId, ChannelType.Facebook, plan.MaxFacebookChannels, cancellationToken);
            await SyncChannelsAsync(tenantId, ChannelType.Instagram, plan.MaxInstagramChannels, cancellationToken);
            await SyncChannelsAsync(tenantId, ChannelType.Telegram, plan.MaxTelegramChannels, cancellationToken);

            // PHASE 2: Apply total channel limits fairly across all types (pass null for channelType)
            await SyncChannelsAsync(tenantId, null, plan.MaxChannels, cancellationToken);

            _logger.LogInformation("Successfully completed comprehensive entity sync for tenant {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync entities with plan limits for tenant {TenantId}", tenantId);
            throw;
        }
    }

    /// <summary>
    /// Syncs users with plan limits - suspends excess users or restores suspended users within limits
    /// </summary>
    private async Task SyncTenantUsersWithPlanLimitsAsync(int tenantId, int maxUsers, CancellationToken cancellationToken)
    {
        // ignore query filters to get all users (active and suspended)
        var allTenantUsers = await _context.TenantUsers.IgnoreQueryFilters().Where(tu => tu.TenantId == tenantId && tu.Status == TenantUserStatus.Active)
            .OrderBy(tu => tu.Created).ToListAsync(cancellationToken); // Oldest users get priority to stay active

        var activeTenantUsers = allTenantUsers.Where(u => u.Status == TenantUserStatus.Active).ToList();
        var suspendedTenantUsers = allTenantUsers.Where(u => u.EntityStatus == EntityStatus.Suspended).ToList();

        _logger.LogDebug("Current user state for tenant {TenantId}: {ActiveCount} active, {SuspendedCount} suspended, limit: {MaxUsers}", tenantId, activeTenantUsers.Count, suspendedTenantUsers.Count, maxUsers);

        // If we have more active users than allowed, suspend the excess (newest first)
        if (activeTenantUsers.Count > maxUsers)
        {
            var tenantUsersToSuspend = activeTenantUsers.OrderByDescending(u => u.Created).Take(activeTenantUsers.Count - maxUsers).ToList();

            foreach (var tenantUser in tenantUsersToSuspend)
            {
                tenantUser.EntityStatus = EntityStatus.Suspended;
                tenantUser.SuspendedAt = DateTimeOffset.UtcNow;
                tenantUser.AddDomainEvent(new TenantUserSuspendedEvent(tenantId, tenantUser.ApplicationUserId, tenantUser.Id));
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Suspended {Count} users for tenant {TenantId} to comply with plan limit of {MaxUsers}", tenantUsersToSuspend.Count, tenantId, maxUsers);
        }
        // If we have room for more users and have suspended users, restore them (oldest first)
        else if (activeTenantUsers.Count < maxUsers && suspendedTenantUsers.Any())
        {
            var availableSlots = maxUsers - activeTenantUsers.Count;
            var tenantUsersToRestore = suspendedTenantUsers.OrderBy(u => u.Created).Take(availableSlots).ToList();

            foreach (var tenantUser in tenantUsersToRestore)
            {
                tenantUser.EntityStatus = EntityStatus.Active;
                tenantUser.ResumedAt = DateTimeOffset.UtcNow;
                tenantUser.AddDomainEvent(new TenantUserRestoredEvent(tenantId, tenantUser.ApplicationUserId, tenantUser.Id));
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Restored {Count} users for tenant {TenantId}. Current active: {ActiveCount}/{MaxUsers}", tenantUsersToRestore.Count, tenantId, activeTenantUsers.Count + tenantUsersToRestore.Count, maxUsers);
        }
        else
        {
            _logger.LogDebug("User limits are already in compliance for tenant {TenantId}: {ActiveCount}/{MaxUsers}", tenantId, activeTenantUsers.Count, maxUsers);
        }
    }

    /// <summary>
    /// Unified method to sync channels - if channelType is null, syncs total channels across all types; otherwise syncs specific channel type
    /// </summary>
    private async Task SyncChannelsAsync(int tenantId, ChannelType? channelType, int maxChannels, CancellationToken cancellationToken)
    {
        // ignore query filters to get all channels (active and suspended) and filter out soft deleted ones
        var query = _context.ChannelAccounts.IgnoreQueryFilters().Where(c => c.TenantId == tenantId && !c.IsDeleted);

        if (channelType.HasValue)
        {
            query = query.Where(c => c.Type == channelType.Value);
        }

        var channels = await query.OrderBy(c => c.Created).ToListAsync(cancellationToken);

        var activeChannels = channels.Where(c => c.EntityStatus == EntityStatus.Active).ToList();
        var suspendedChannels = channels.Where(c => c.EntityStatus == EntityStatus.Suspended).ToList();

        // Logging - different messages for type vs total
        var logContext = channelType.HasValue ? $"{channelType.Value} channel" : "Total channel";
        _logger.LogDebug("{LogContext} state for tenant {TenantId}: {ActiveCount} active, {SuspendedCount} suspended, limit: {MaxChannels}", logContext, tenantId, activeChannels.Count, suspendedChannels.Count, maxChannels);

        // SUSPENSION: If exceeds limit, suspend excess (newest first)
        if (activeChannels.Count > maxChannels)
        {
            var channelsToSuspend = activeChannels.OrderByDescending(c => c.Created).Take(activeChannels.Count - maxChannels).ToList();

            foreach (var channel in channelsToSuspend)
            {
                channel.EntityStatus = EntityStatus.Suspended;
                channel.SuspendedAt = DateTimeOffset.UtcNow;
                channel.AddDomainEvent(new ChannelAccountSuspendedEvent(tenantId, channel.Id));
            }

            await _context.SaveChangesAsync(cancellationToken);

            var suspensionsByType = channelsToSuspend.GroupBy(c => c.Type).ToDictionary(g => g.Key, g => g.Count());
            var logMessage = string.Join(", ", suspensionsByType.Select(kvp => $"{kvp.Value} {kvp.Key}"));
            _logger.LogInformation("Suspended {Count} channels for tenant {TenantId} to comply with total limit of {MaxChannels}: {ByType}",
                channelsToSuspend.Count, tenantId, maxChannels, logMessage);
        }
        // RESTORATION: If under limit and have suspended channels, restore them (oldest first)
        else if (activeChannels.Count < maxChannels && suspendedChannels.Any())
        {
            var availableSlots = maxChannels - activeChannels.Count;
            var channelsToRestore = suspendedChannels.OrderBy(c => c.Created).Take(availableSlots).ToList();

            foreach (var channel in channelsToRestore)
            {
                channel.EntityStatus = EntityStatus.Active;
                channel.ResumedAt = DateTimeOffset.UtcNow;
                channel.AddDomainEvent(new ChannelAccountRestoredEvent(tenantId, channel.Id));
            }

            await _context.SaveChangesAsync(cancellationToken);

            var restorationsByType = channelsToRestore.GroupBy(c => c.Type).ToDictionary(g => g.Key, g => g.Count());
            var logMessage = string.Join(", ", restorationsByType.Select(kvp => $"{kvp.Value} {kvp.Key}"));
            _logger.LogInformation("Restored {Count} channels for tenant {TenantId} for total limit compliance. Current total active: {TotalActive}/{MaxChannels}: {ByType}",
                channelsToRestore.Count, tenantId, activeChannels.Count + channelsToRestore.Count, maxChannels, logMessage);
        }
        else
        {
            _logger.LogDebug("{LogContext} limits are in compliance for tenant {TenantId}: {ActiveCount}/{MaxChannels}",
                logContext, tenantId, activeChannels.Count, maxChannels);
        }
    }    /// <summary>
         /// Suspends all entities for a tenant when there's no active subscription
         /// </summary>
    private async Task SuspendAllEntitiesAsync(int tenantId, string reason, CancellationToken cancellationToken)
    {
        // query filter will filter out suspended users
        var activeUsers = await _context.TenantUsers.Where(tu => tu.TenantId == tenantId && tu.EntityStatus == EntityStatus.Active).ToListAsync(cancellationToken);

        foreach (var user in activeUsers)
        {
            user.EntityStatus = EntityStatus.Suspended;
            user.SuspendedAt = DateTimeOffset.UtcNow;
            user.AddDomainEvent(new TenantUserSuspendedEvent(tenantId, user.ApplicationUserId, user.Id));
        }

        // ignore query filters and manually include active channels and non deleted ones
        var activeChannels = await _context.ChannelAccounts
            .IgnoreQueryFilters()
            .Where(c => c.TenantId == tenantId && !c.IsDeleted && c.EntityStatus == EntityStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var channel in activeChannels)
        {
            channel.EntityStatus = EntityStatus.Suspended;
            channel.SuspendedAt = DateTimeOffset.UtcNow;
            channel.AddDomainEvent(new ChannelAccountSuspendedEvent(tenantId, channel.Id));
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Suspended all entities for tenant {TenantId}: {UserCount} users, {ChannelCount} channels. Reason: {Reason}",
            tenantId, activeUsers.Count, activeChannels.Count, reason);
    }

    #endregion

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
        var applicationUserId = _contextManager.GetCurrentApplicationUserId();
        if (!applicationUserId.HasValue)
        {
            throw new UnauthorizedAccessException();
        }
        return applicationUserId.Value;
    }

    private bool IsSuperAdminAllowed(bool allowSuperAdmin)
    {
        return allowSuperAdmin && _contextManager.IsSuperAdmin();
    }

    #endregion
}
