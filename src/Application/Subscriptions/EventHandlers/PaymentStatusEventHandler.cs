using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Constants;

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
                subscription.PaymentRetryCount = 0;
                subscription.FirstPaymentFailureAt = null;
                subscription.LastPaymentFailedAt = null;
                subscription.NextRetryAt = null;
                subscription.HasReachedMaxRetries = false;
                subscription.IsInGracePeriod = false;
                subscription.GracePeriodEndsAt = null;

                // Reactivate if suspended
                if (subscription.Status == SubscriptionStatus.PastDue || subscription.Status == SubscriptionStatus.Unpaid)
                {
                    subscription.Status = SubscriptionStatus.Active;
                }

                // Trigger reactivation if subscription was suspended
                if (originalStatus == SubscriptionStatus.Unpaid)
                {
                    TriggerSubscriptionReactivationAsync(subscription, "Payment successful");
                }
                break;

            case PaymentAction.Failed:
                subscription.PaymentRetryCount = notification.FailureCount;
                subscription.LastPaymentFailedAt = notification.Timestamp;

                if (subscription.FirstPaymentFailureAt == null)
                {
                    subscription.FirstPaymentFailureAt = notification.Timestamp;
                }

                // Calculate next retry time based on Stripe's retry schedule
                subscription.NextRetryAt = CalculateNextRetryTime(notification.FailureCount, subscription.FirstPaymentFailureAt.Value);

                // Start grace period if configured
                if (!subscription.IsInGracePeriod && ShouldStartGracePeriod(notification.FailureCount))
                {
                    subscription.IsInGracePeriod = true;
                    subscription.GracePeriodEndsAt = _subscriptionSettings.UseIntelligentGracePeriod
                        ? CalculateIntelligentGracePeriodEnd(notification.FailureCount, subscription.FirstPaymentFailureAt.Value)
                        : notification.Timestamp.AddDays(_subscriptionSettings.GracePeriodDays);
                    subscription.Status = SubscriptionStatus.PastDue;

                    // Trigger grace period start
                    TriggerGracePeriodStartAsync(subscription, notification.Reason);
                }

                // Check if suspension is needed
                if (ShouldSuspendAfterFailure(notification.FailureCount))
                {
                    TriggerSubscriptionSuspensionAsync(subscription, notification.Reason);
                }
                break;

            case PaymentAction.Retry:
                subscription.PaymentRetryCount = notification.FailureCount;
                subscription.NextRetryAt = notification.Timestamp.AddHours(24); // Next retry in 24 hours
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated payment tracking for subscription {SubscriptionId}: {Action}",
            subscription.Id, notification.Action);
    }

    private void TriggerSubscriptionSuspensionAsync(Subscription subscription, string reason)
    {
        var suspensionEvent = new SubscriptionStatusEvent(subscription.TenantId, default, subscription, SubscriptionAction.Suspend, reason, sendEmailNotification: true);

        subscription.AddDomainEvent(suspensionEvent);

        _logger.LogInformation("Triggered suspension for subscription {SubscriptionId}", subscription.Id);
    }

    private void TriggerSubscriptionReactivationAsync(Subscription subscription, string reason)
    {
        var reactivationEvent = new SubscriptionStatusEvent(subscription.TenantId, default, subscription, SubscriptionAction.Reactivate, reason, sendEmailNotification: true);

        subscription.AddDomainEvent(reactivationEvent);

        _logger.LogInformation("Triggered reactivation for subscription {SubscriptionId}", subscription);
    }

    private void TriggerGracePeriodStartAsync(Subscription subscription, string reason)
    {
        var gracePeriodEvent = new SubscriptionStatusEvent(subscription.TenantId, default, subscription, SubscriptionAction.GracePeriodStart, reason, sendEmailNotification: true);

        subscription.AddDomainEvent(gracePeriodEvent);

        _logger.LogInformation("Triggered grace period start for subscription {SubscriptionId}", subscription.Id);
    }

    private async Task SendEmailNotificationAsync(Subscription subscription, PaymentStatusEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var templateId = GetEmailTemplateId(notification.Action);
            var subject = GetEmailSubject(notification.Action);

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
                    { "timestamp", notification.Timestamp.ToString("MMMM dd, yyyy") },
                    { "nextRetryDate", subscription.NextRetryAt?.ToString("MMMM dd, yyyy") ?? string.Empty },
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

    private bool ShouldStartGracePeriod(int failureCount)
    {
        // Start grace period on first failure (can be configured)
        return failureCount >= 1;
    }

    private bool ShouldSuspendAfterFailure(int failureCount)
    {
        // Suspend after max retries reached
        return failureCount >= _subscriptionSettings.MaxPaymentRetries;
    }

    private DateTimeOffset CalculateNextRetryTime(int attemptCount, DateTimeOffset firstFailureAt)
    {
        // Stripe's approximate retry schedule:
        // 1st retry: 3-5 days after initial failure
        // 2nd retry: 5-7 days after first retry
        // 3rd retry: 7-9 days after second retry
        // 4th retry: 10-11 days after third retry

        return attemptCount switch
        {
            1 => firstFailureAt.AddDays(4), // First retry around day 4
            2 => firstFailureAt.AddDays(9), // Second retry around day 9
            3 => firstFailureAt.AddDays(16), // Third retry around day 16
            4 => firstFailureAt.AddDays(25), // Final retry around day 25
            _ => firstFailureAt.AddDays(3) // Default fallback
        };
    }

    private DateTimeOffset CalculateIntelligentGracePeriodEnd(int attemptCount, DateTimeOffset firstFailureAt)
    {
        // Base grace period calculation
        var baseGracePeriodEnd = DateTimeOffset.UtcNow.AddDays(_subscriptionSettings.GracePeriodDays);

        // If we're still in Stripe's retry period, extend grace period beyond the last expected retry
        var nextRetryTime = CalculateNextRetryTime(attemptCount, firstFailureAt);
        var stripeRetryEndTime = firstFailureAt.AddDays(_subscriptionSettings.StripeRetryPeriodDays);

        // Ensure grace period extends beyond Stripe's retry attempts with additional buffer
        var intelligentEndTime = stripeRetryEndTime.AddHours(_subscriptionSettings.RetryAttemptGracePeriodHours);

        // Use the later of the two dates
        return baseGracePeriodEnd > intelligentEndTime ? baseGracePeriodEnd : intelligentEndTime;
    }

    private string GetEmailTemplateId(PaymentAction action) => action switch
    {
        PaymentAction.Success => EmailTemplates.PaymentSuccess,
        PaymentAction.Failed => EmailTemplates.PaymentFailed,
        PaymentAction.Retry => EmailTemplates.PaymentRetry,
        _ => EmailTemplates.PaymentFailed
    };

    private string GetEmailSubject(PaymentAction action) => action switch
    {
        PaymentAction.Success => "Payment Successful - Thank You!",
        PaymentAction.Failed => "Payment Failed - Action Required",
        PaymentAction.Retry => "Payment Retry Scheduled",
        _ => "Payment Update"
    };
}