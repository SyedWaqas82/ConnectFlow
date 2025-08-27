using ConnectFlow.Domain.Events.Mediator.Subscriptions;
using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Subscriptions.EventHandlers;

/// <summary>
/// Handles payment status changes and triggers subscription actions
/// </summary>
public class PaymentStatusEventHandler : INotificationHandler<PaymentStatusEvent>
{
    private readonly ILogger<PaymentStatusEventHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly SubscriptionSettings _subscriptionSettings;

    public PaymentStatusEventHandler(ILogger<PaymentStatusEventHandler> logger, IApplicationDbContext context, IEmailService emailService, IOptions<SubscriptionSettings> subscriptionSettings)
    {
        _logger = logger;
        _context = context;
        _emailService = emailService;
        _subscriptionSettings = subscriptionSettings.Value;
    }

    public async Task Handle(PaymentStatusEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment status event: {Action} for subscription {SubscriptionId}",
            notification.Action, notification.SubscriptionId);

        try
        {
            var subscription = await _context.Subscriptions.Include(s => s.Tenant).FirstOrDefaultAsync(s => s.Id == notification.SubscriptionId, cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found", notification.SubscriptionId);
                return;
            }

            // Update payment tracking and handle payment actions
            await UpdatePaymentTrackingAsync(subscription, notification, cancellationToken);

            // Send email notification if required
            if (notification.SendEmailNotification)
            {
                await SendEmailNotificationAsync(subscription, notification, cancellationToken);
            }

            _logger.LogInformation("Successfully processed payment status event for {SubscriptionId}", notification.SubscriptionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process payment status event for {SubscriptionId}",
                notification.SubscriptionId);
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
                    TriggerSubscriptionReactivationAsync(subscription, "Payment successful", cancellationToken);
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
                    TriggerGracePeriodStartAsync(subscription, notification.Reason, cancellationToken);
                }

                // Check if suspension is needed
                if (ShouldSuspendAfterFailure(notification.FailureCount))
                {
                    TriggerSubscriptionSuspensionAsync(subscription, notification.Reason, cancellationToken);
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

    private void TriggerSubscriptionSuspensionAsync(Subscription subscription, string reason, CancellationToken cancellationToken)
    {
        var suspensionEvent = new SubscriptionStatusEvent(
            subscription.Id,
            SubscriptionAction.Suspend,
            reason,
            sendEmailNotification: true,
            tenantId: subscription.TenantId);

        subscription.AddDomainEvent(suspensionEvent);

        _logger.LogInformation("Triggered suspension for subscription {SubscriptionId}", subscription.Id);
    }

    private void TriggerSubscriptionReactivationAsync(Subscription subscription, string reason, CancellationToken cancellationToken)
    {
        var reactivationEvent = new SubscriptionStatusEvent(
            subscription.Id,
            SubscriptionAction.Reactivate,
            reason,
            sendEmailNotification: true,
            tenantId: subscription.TenantId);

        subscription.AddDomainEvent(reactivationEvent);

        _logger.LogInformation("Triggered reactivation for subscription {SubscriptionId}", subscription.Id);
    }

    private void TriggerGracePeriodStartAsync(Subscription subscription, string reason, CancellationToken cancellationToken)
    {
        var gracePeriodEvent = new SubscriptionStatusEvent(
            subscriptionId: subscription.Id,
            action: SubscriptionAction.GracePeriodStart,
            reason: reason,
            sendEmailNotification: true,
            suspendLimitsImmediately: false, // Grace period is lenient
            tenantId: subscription.TenantId);

        subscription.AddDomainEvent(gracePeriodEvent);

        _logger.LogInformation("Triggered grace period start for subscription {SubscriptionId}", subscription.Id);
    }

    private async Task SendEmailNotificationAsync(Subscription subscription, PaymentStatusEvent notification, CancellationToken cancellationToken)
    {
        var subject = GetEmailSubject(notification.Action);
        var body = GetEmailBody(notification.Action, subscription.Tenant.Name, notification.Reason, notification.Amount);

        var emailMessage = new EmailMessage
        {
            To = subscription.Tenant.Email ?? string.Empty,
            Subject = subject,
            Body = body
        };

        var result = await _emailService.SendAsync(emailMessage, cancellationToken);

        if (result.Succeeded)
        {
            _logger.LogInformation("Sent {Action} email to {Email} for subscription {SubscriptionId}",
                notification.Action, subscription.Tenant.Email, subscription.Id);
        }
        else
        {
            _logger.LogError("Failed to send email to {Email} for subscription {SubscriptionId}: {Errors}",
                subscription.Tenant.Email, subscription.Id, string.Join(", ", result.Errors));
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

    private string GetEmailSubject(PaymentAction action) => action switch
    {
        PaymentAction.Success => "Payment Successful - Thank You!",
        PaymentAction.Failed => "Payment Failed - Action Required",
        PaymentAction.Retry => "Payment Retry Scheduled",
        PaymentAction.Refunded => "Payment Refunded",
        PaymentAction.PartialRefund => "Partial Refund Processed",
        _ => "Payment Update"
    };

    private string GetEmailBody(PaymentAction action, string tenantName, string reason, decimal? amount) => action switch
    {
        PaymentAction.Success => $"Hello {tenantName},\n\nYour payment of {amount:C} has been processed successfully. Thank you for your continued subscription!",
        PaymentAction.Failed => $"Hello {tenantName},\n\nWe were unable to process your payment. Reason: {reason}\n\nPlease update your payment method to avoid service interruption.",
        PaymentAction.Retry => $"Hello {tenantName},\n\nWe will retry processing your payment. Please ensure your payment method is up to date.",
        PaymentAction.Refunded => $"Hello {tenantName},\n\nA refund of {amount:C} has been processed. Reason: {reason}",
        PaymentAction.PartialRefund => $"Hello {tenantName},\n\nA partial refund of {amount:C} has been processed. Reason: {reason}",
        _ => $"Hello {tenantName},\n\nYour payment status has been updated. Amount: {amount:C}"
    };
}
