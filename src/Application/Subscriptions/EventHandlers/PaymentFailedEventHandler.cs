namespace ConnectFlow.Application.Subscriptions.EventHandlers;

public class PaymentFailedEventHandler : INotificationHandler<PaymentFailedEvent>
{
    private readonly ILogger<PaymentFailedEventHandler> _logger;
    private readonly IEmailService _emailService;

    public PaymentFailedEventHandler(ILogger<PaymentFailedEventHandler> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(PaymentFailedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling payment failed event for tenant {TenantId}, subscription {SubscriptionId}, invoice {InvoiceId}",
            notification.TenantId, notification.SubscriptionId, notification.InvoiceId);

        // Send payment failure notification
        try
        {
            // You can implement email sending logic here
            // await _emailService.SendPaymentFailedEmailAsync(notification.TenantId, notification.Amount, notification.Currency, notification.FailureReason, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send payment failed email for subscription {SubscriptionId}", notification.SubscriptionId);
        }

        await Task.CompletedTask;
    }
}