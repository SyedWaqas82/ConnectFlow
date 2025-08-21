namespace ConnectFlow.Application.Subscriptions.EventHandlers;

public class SubscriptionUpgradedEventHandler : INotificationHandler<SubscriptionUpgradedEvent>
{
    private readonly ILogger<SubscriptionUpgradedEventHandler> _logger;
    private readonly IEmailService _emailService;

    public SubscriptionUpgradedEventHandler(ILogger<SubscriptionUpgradedEventHandler> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(SubscriptionUpgradedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling subscription upgraded event for tenant {TenantId}, subscription {SubscriptionId}, from {FromPlan} to {ToPlan}",
            notification.TenantId, notification.SubscriptionId, notification.FromPlan, notification.ToPlan);

        // Send upgrade confirmation email
        try
        {
            // You can implement email sending logic here
            // await _emailService.SendSubscriptionUpgradeEmailAsync(notification.TenantId, notification.FromPlan, notification.ToPlan, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send upgrade email for subscription {SubscriptionId}", notification.SubscriptionId);
        }

        await Task.CompletedTask;
    }
}