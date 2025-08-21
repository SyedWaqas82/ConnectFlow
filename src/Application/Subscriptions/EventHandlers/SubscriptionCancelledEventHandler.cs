namespace ConnectFlow.Application.Subscriptions.EventHandlers;

public class SubscriptionCancelledEventHandler : INotificationHandler<SubscriptionCancelledEvent>
{
    private readonly ILogger<SubscriptionCancelledEventHandler> _logger;
    private readonly IEmailService _emailService;

    public SubscriptionCancelledEventHandler(ILogger<SubscriptionCancelledEventHandler> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(SubscriptionCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling subscription cancelled event for tenant {TenantId}, subscription {SubscriptionId}, immediate: {Immediate}",
            notification.TenantId, notification.SubscriptionId, notification.ImmediateCancellation);

        // Send cancellation email
        try
        {
            // You can implement email sending logic here
            // await _emailService.SendSubscriptionCancellationEmailAsync(notification.TenantId, notification.ImmediateCancellation, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send cancellation email for subscription {SubscriptionId}", notification.SubscriptionId);
        }

        await Task.CompletedTask;
    }
}