using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Subscriptions.EventHandlers;

/// <summary>
/// Handles subscription status changes - suspends/reactivates subscriptions, manages limits, and sends notifications
/// </summary>
public class SubscriptionStatusEventHandler : INotificationHandler<SubscriptionStatusEvent>
{
    private readonly ILogger<SubscriptionStatusEventHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ISubscriptionManagementService _subscriptionManagementService;
    private readonly SubscriptionSettings _subscriptionSettings;

    public SubscriptionStatusEventHandler(ILogger<SubscriptionStatusEventHandler> logger, IApplicationDbContext context, IEmailService emailService, ISubscriptionManagementService subscriptionManagementService, IOptions<SubscriptionSettings> subscriptionSettings)
    {
        _logger = logger;
        _context = context;
        _emailService = emailService;
        _subscriptionManagementService = subscriptionManagementService;
        _subscriptionSettings = subscriptionSettings.Value;
    }

    public async Task Handle(SubscriptionStatusEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing subscription status event: {Action} for subscription {SubscriptionId} (Immediate: {SuspendLimitsImmediately})",
            notification.Action, notification.SubscriptionId, notification.SuspendLimitsImmediately);

        try
        {
            var subscription = await _context.Subscriptions.Include(s => s.Tenant).FirstOrDefaultAsync(s => s.Id == notification.SubscriptionId, cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found", notification.SubscriptionId);
                return;
            }

            // Update subscription status
            await UpdateSubscriptionStatusAsync(subscription, notification, cancellationToken);

            // Handle subscription limits based on action
            await HandleSubscriptionLimitsAsync(subscription, notification, cancellationToken);

            // Send email notification if required
            if (notification.SendEmailNotification)
            {
                await SendEmailNotificationAsync(subscription, notification, cancellationToken);
            }

            _logger.LogInformation("Successfully processed subscription status event for {SubscriptionId}",
                notification.SubscriptionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process subscription status event for {SubscriptionId}",
                notification.SubscriptionId);
            throw;
        }
    }

    private async Task UpdateSubscriptionStatusAsync(Subscription subscription, SubscriptionStatusEvent notification, CancellationToken cancellationToken)
    {
        switch (notification.Action)
        {
            case SubscriptionAction.Create:
                // No status update needed for creation - already set
                _logger.LogInformation("Subscription {SubscriptionId} created", subscription.Id);
                break;

            case SubscriptionAction.Suspend:
                subscription.Status = SubscriptionStatus.Unpaid;
                break;

            case SubscriptionAction.Reactivate:
                subscription.Status = SubscriptionStatus.Active;
                break;

            case SubscriptionAction.Cancel:
                subscription.Status = SubscriptionStatus.Canceled;
                subscription.CanceledAt = notification.Timestamp;
                subscription.CancelAtPeriodEnd = false;
                break;

            case SubscriptionAction.GracePeriodStart:
                subscription.Status = SubscriptionStatus.PastDue;
                subscription.IsInGracePeriod = true;
                subscription.GracePeriodEndsAt = notification.Timestamp.AddDays(_subscriptionSettings.GracePeriodDays);
                break;

            case SubscriptionAction.GracePeriodEnd:
                subscription.IsInGracePeriod = false;
                subscription.GracePeriodEndsAt = null;
                break;

            case SubscriptionAction.PlanChanged:
                // Plan changes are handled in webhook - just log here
                _logger.LogInformation("Plan change processed for subscription {SubscriptionId}", subscription.Id);
                break;

            case SubscriptionAction.StatusUpdate:
                // Generic status update - no specific action needed
                _logger.LogInformation("Status update processed for subscription {SubscriptionId}", subscription.Id);
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated subscription {SubscriptionId} for action {Action}",
            subscription.Id, notification.Action);
    }

    private async Task HandleSubscriptionLimitsAsync(Subscription subscription, SubscriptionStatusEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Use the subscription management service to sync entities with their limits
            await _subscriptionManagementService.SyncEntitiesWithLimitsAsync(subscription.TenantId, cancellationToken);

            _logger.LogInformation("Successfully synced entities with limits for action {Action} - Subscription {SubscriptionId}",
                notification.Action, subscription.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync entities with limits for action {Action} - Subscription {SubscriptionId}",
                notification.Action, subscription.Id);
            // Don't throw - this shouldn't fail the main subscription status update
        }
    }

    private async Task SendEmailNotificationAsync(Subscription subscription, SubscriptionStatusEvent notification, CancellationToken cancellationToken)
    {
        var subject = GetEmailSubject(notification.Action, notification.SuspendLimitsImmediately);
        var body = GetEmailBody(notification.Action, subscription.Tenant.Name, notification.Reason, notification.SuspendLimitsImmediately);

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

    private string GetEmailSubject(SubscriptionAction action, bool suspendLimitsImmediately = false) => action switch
    {
        SubscriptionAction.Create => "Welcome! Your subscription is now active",
        SubscriptionAction.Suspend => "Your subscription has been suspended",
        SubscriptionAction.Reactivate => "Your subscription has been reactivated",
        SubscriptionAction.Cancel when suspendLimitsImmediately => "Your subscription has been cancelled immediately",
        SubscriptionAction.Cancel => "Your subscription will be cancelled at period end",
        SubscriptionAction.GracePeriodStart => "Payment required - Grace period started",
        SubscriptionAction.GracePeriodEnd => "Grace period ended",
        SubscriptionAction.PlanChanged => "Your subscription plan has been updated",
        _ => "Subscription update"
    };

    private string GetEmailBody(SubscriptionAction action, string tenantName, string reason, bool suspendLimitsImmediately = false) => action switch
    {
        SubscriptionAction.Create => $"Hello {tenantName},\n\nWelcome! Your subscription is now active and you have full access to our services. Thank you for joining us!",
        SubscriptionAction.Suspend => $"Hello {tenantName},\n\nYour subscription has been suspended. Reason: {reason}\n\nPlease contact support if you have any questions.",
        SubscriptionAction.Reactivate => $"Hello {tenantName},\n\nGreat news! Your subscription has been reactivated and you now have full access to our services.",
        SubscriptionAction.Cancel when suspendLimitsImmediately => $"Hello {tenantName},\n\nYour subscription has been cancelled immediately. Reason: {reason}\n\nThank you for using our services.",
        SubscriptionAction.Cancel => $"Hello {tenantName},\n\nYour subscription is scheduled to be cancelled at the end of your current billing period. Reason: {reason}\n\nYou'll continue to have access until your current period ends.",
        SubscriptionAction.GracePeriodStart => $"Hello {tenantName},\n\nWe were unable to process your payment. You have {(_subscriptionSettings?.GracePeriodDays ?? 7)} days to update your payment method before your service is suspended.",
        SubscriptionAction.GracePeriodEnd => $"Hello {tenantName},\n\nYour grace period has ended. Please update your payment method to continue using our services.",
        SubscriptionAction.PlanChanged => $"Hello {tenantName},\n\nYour subscription plan has been updated. {reason}",
        _ => $"Hello {tenantName},\n\nYour subscription has been updated. {reason}"
    };
}
