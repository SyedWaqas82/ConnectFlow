using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Common.Messaging.Handlers;

public class SubscriptionMessageEventHandler : IMessageHandler<SubscriptionMessageEvent>
{
    private readonly ISubscriptionManagementService _subscriptionManagementService;
    private readonly ILogger<SubscriptionMessageEventHandler> _logger;
    private readonly MessagingConfiguration.Queue _queue;

    public SubscriptionMessageEventHandler(ISubscriptionManagementService subscriptionManagementService, ILogger<SubscriptionMessageEventHandler> logger)
    {
        _subscriptionManagementService = subscriptionManagementService;
        _logger = logger;
        _queue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Subscription);
    }

    public async Task HandleAsync(SubscriptionMessageEvent message, CancellationToken cancellationToken = default)
    {
        try
        {
            await _subscriptionManagementService.SyncEntitiesWithLimitsAsync(message.TenantId, cancellationToken);

            _logger.LogInformation("Successfully synced entities with limits for tenant {TenantId}", message.TenantId);

            message.Acknowledge();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing entities with limits for tenant {TenantId}", message.TenantId);
            message.Reject(requeue: message.RetryCount < _queue.MaxRetries);
        }
    }
}