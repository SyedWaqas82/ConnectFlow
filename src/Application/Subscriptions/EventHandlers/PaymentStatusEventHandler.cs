using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Application.Common.Exceptions;

namespace ConnectFlow.Application.Subscriptions.EventHandlers;

/// <summary>
/// Handles payment status changes and triggers subscription actions
/// </summary>
public class PaymentStatusEventHandler : INotificationHandler<PaymentStatusEvent>
{
    private readonly ILogger<PaymentStatusEventHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IMessagePublisher _messagePublisher;
    private readonly SubscriptionSettings _subscriptionSettings;

    public PaymentStatusEventHandler(ILogger<PaymentStatusEventHandler> logger, IApplicationDbContext context, IMessagePublisher messagePublisher, IOptions<SubscriptionSettings> subscriptionSettings)
    {
        _logger = logger;
        _context = context;
        _messagePublisher = messagePublisher;
        _subscriptionSettings = subscriptionSettings.Value;
    }

    public async Task Handle(PaymentStatusEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment status event: {Action} for subscription {SubscriptionId}",
            notification.Action, notification.Subscription.Id);

        try
        {
            var subscription = notification.Subscription;

            // Update payment tracking and handle payment actions
            await UpdatePaymentTrackingAsync(subscription, notification, cancellationToken);

            // Send email notification if required
            if (notification.SendEmailNotification)
            {
                await SendEmailNotificationAsync(subscription, notification, cancellationToken);
            }

            _logger.LogInformation("Successfully processed payment status event for {SubscriptionId}", notification.Subscription.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process payment status event for {SubscriptionId}",
                notification.Subscription.Id);
            throw;
        }
    }

    private async Task UpdatePaymentTrackingAsync(Subscription subscription, PaymentStatusEvent notification, CancellationToken cancellationToken)
    {
        // Capture original status before any changes
        var originalStatus = subscription.Status;

        switch (notification.Action)
        {
            case PaymentAction.Success:
                // Reset failure tracking on successful payment
                subscription.IsInGracePeriod = false;
                subscription.GracePeriodEndsAt = null;
                subscription.FirstPaymentFailureAt = null;
                subscription.LastPaymentFailedAt = null;
                subscription.PaymentRetryCount = 0;
                subscription.HasReachedMaxRetries = false;

                // Reactivate if suspended
                if (subscription.Status == SubscriptionStatus.PastDue || subscription.Status == SubscriptionStatus.Unpaid)
                {
                    subscription.Status = SubscriptionStatus.Active;
                }

                // Trigger reactivation if subscription was suspended
                if (originalStatus == SubscriptionStatus.Unpaid)
                {
                    var reactivationEvent = new SubscriptionStatusEvent(subscription.TenantId, default, subscription, SubscriptionAction.Reactivate, "Payment successful", sendEmailNotification: true);

                    subscription.AddDomainEvent(reactivationEvent);

                    _logger.LogInformation("Triggered reactivation for subscription {SubscriptionId}", subscription);
                }
                break;

            case PaymentAction.Failed:
                subscription.PaymentRetryCount = notification.FailureCount;
                subscription.LastPaymentFailedAt = DateTimeOffset.UtcNow;

                if (subscription.FirstPaymentFailureAt == null)
                {
                    subscription.FirstPaymentFailureAt = subscription.LastPaymentFailedAt;
                }

                // Start grace period if configured
                if (!subscription.IsInGracePeriod && notification.FailureCount >= 1)
                {
                    subscription.IsInGracePeriod = true;
                    subscription.GracePeriodEndsAt = DateTimeOffset.UtcNow.AddDays(_subscriptionSettings.GracePeriodDays);

                    // Trigger grace period start
                    var gracePeriodEvent = new SubscriptionStatusEvent(subscription.TenantId, default, subscription, SubscriptionAction.GracePeriodStart, notification.Reason, sendEmailNotification: true);

                    subscription.AddDomainEvent(gracePeriodEvent);

                    _logger.LogInformation("Triggered grace period start for subscription {SubscriptionId}", subscription.Id);
                }

                // Check if suspension is needed
                if (notification.FailureCount >= _subscriptionSettings.MaxPaymentRetries)
                {
                    var suspensionEvent = new SubscriptionStatusEvent(subscription.TenantId, default, subscription, SubscriptionAction.Suspend, notification.Reason, sendEmailNotification: true);

                    subscription.AddDomainEvent(suspensionEvent);

                    _logger.LogInformation("Triggered suspension for subscription {SubscriptionId}", subscription.Id);
                }
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated payment tracking for subscription {SubscriptionId}: {Action}", subscription.Id, notification.Action);
    }

    private async Task SendEmailNotificationAsync(Subscription subscription, PaymentStatusEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var templateId = GetEmailTemplateId(notification.Action);
            var subject = GetEmailSubject(notification.Action);

            //check if subscription does not have tenant loaded then load now
            if (subscription.Tenant == null)
            {
                subscription.Tenant = await _context.Tenants.FindAsync(new object[] { subscription.TenantId }, cancellationToken) ?? throw new TenantNotFoundException($"Tenant not found for subscription {subscription.Id}");
            }

            var emailEvent = new EmailSendMessageEvent(notification.TenantId, notification.ApplicationUserId)
            {
                CorrelationId = notification.CorrelationId,
                ApplicationUserPublicId = notification.ApplicationUserPublicId,
                To = subscription.Tenant.Email,
                Subject = subject,
                IsHtml = true,
                TemplateId = templateId,
                TemplateData = new Dictionary<string, object>
                {
                    { "tenantName", subscription.Tenant.Name ?? "Valued Customer" },
                    { "amount", notification.Amount?.ToString("C") ?? "$0.00" },
                    { "reason", notification.Reason },
                    { "subscriptionId", subscription.Id },
                    { "planName", subscription.Plan?.Name ?? "Current Plan" },
                    { "paymentAction", notification.Action.ToString() },
                    { "timestamp", DateTimeOffset.UtcNow.ToString("MMMM dd, yyyy") },
                    { "failureCount", notification.FailureCount },
                    { "correlationId", notification.CorrelationId.GetValueOrDefault() }
                },
            };

            var queue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Email);
            await _messagePublisher.PublishAsync(emailEvent, queue.RoutingKey, cancellationToken);

            _logger.LogInformation("Queued {Action} email notification for tenant {TenantEmail} for subscription {SubscriptionId}", notification.Action, subscription.Tenant.Email, subscription.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue email notification for subscription {SubscriptionId}", subscription.Id);
        }
    }

    private string GetEmailTemplateId(PaymentAction action) => action switch
    {
        PaymentAction.Success => EmailTemplates.PaymentSuccess,
        PaymentAction.Failed => EmailTemplates.PaymentFailed,
        _ => EmailTemplates.PaymentFailed
    };

    private string GetEmailSubject(PaymentAction action) => action switch
    {
        PaymentAction.Success => "Payment Successful - Thank You!",
        PaymentAction.Failed => "Payment Failed - Action Required",
        _ => "Payment Update"
    };
}