namespace ConnectFlow.Application.Subscriptions.EventHandlers;

public class SubscriptionDowngradedEventHandler : INotificationHandler<SubscriptionDowngradedEvent>
{
    private readonly ILogger<SubscriptionDowngradedEventHandler> _logger;
    private readonly IEmailService _emailService;

    public SubscriptionDowngradedEventHandler(ILogger<SubscriptionDowngradedEventHandler> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(SubscriptionDowngradedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling subscription downgraded event for tenant {TenantId}, subscription {SubscriptionId}, from {FromPlan} to {ToPlan}",
            notification.TenantId, notification.SubscriptionId, notification.FromPlan, notification.ToPlan);

        // Send downgrade confirmation email
        try
        {
            // You can implement email sending logic here
            // await _emailService.SendSubscriptionDowngradeEmailAsync(notification.TenantId, notification.FromPlan, notification.ToPlan, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send downgrade email for subscription {SubscriptionId}", notification.SubscriptionId);
        }

        await Task.CompletedTask;
    }
}