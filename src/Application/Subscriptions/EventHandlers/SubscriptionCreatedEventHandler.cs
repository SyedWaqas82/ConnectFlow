namespace ConnectFlow.Application.Subscriptions.EventHandlers;

public class SubscriptionCreatedEventHandler : INotificationHandler<SubscriptionCreatedEvent>
{
    private readonly ILogger<SubscriptionCreatedEventHandler> _logger;
    private readonly IEmailService _emailService;

    public SubscriptionCreatedEventHandler(ILogger<SubscriptionCreatedEventHandler> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(SubscriptionCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling subscription created event for tenant {TenantId}, subscription {SubscriptionId}",
            notification.TenantId, notification.SubscriptionId);

        // Send welcome email
        try
        {
            // You can implement email sending logic here
            // await _emailService.SendSubscriptionWelcomeEmailAsync(notification.TenantId, notification.PlanName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email for subscription {SubscriptionId}", notification.SubscriptionId);
        }

        // Perform any other initialization tasks
        await Task.CompletedTask;
    }
}