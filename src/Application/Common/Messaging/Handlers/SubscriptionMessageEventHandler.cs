namespace ConnectFlow.Application.Common.Messaging.Handlers;

public class SubscriptionMessageEventHandler : IMessageHandler<SubscriptionMessageEvent>
{
    private readonly ISubscriptionManagementService _subscriptionManagementService;
    private readonly ILogger<SubscriptionMessageEventHandler> _logger;

    public SubscriptionMessageEventHandler(ISubscriptionManagementService subscriptionManagementService, ILogger<SubscriptionMessageEventHandler> logger)
    {
        _subscriptionManagementService = subscriptionManagementService;
        _logger = logger;
    }

    public async Task HandleAsync(SubscriptionMessageEvent message, CancellationToken cancellationToken = default)
    {
        await _subscriptionManagementService.SyncEntitiesWithLimitsAsync(message.TenantId, cancellationToken);

        _logger.LogInformation("Successfully synced entities with limits for tenant {TenantId}", message.TenantId);
    }
}