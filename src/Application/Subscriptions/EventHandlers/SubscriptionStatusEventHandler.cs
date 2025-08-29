using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.EventHandlers;

/// <summary>
/// Handles subscription status changes - suspends/reactivates subscriptions, manages limits, and sends notifications
/// </summary>
public class SubscriptionStatusEventHandler : INotificationHandler<SubscriptionStatusEvent>
{
    private readonly ILogger<SubscriptionStatusEventHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ISubscriptionManagementService _subscriptionManagementService;
    private readonly SubscriptionSettings _subscriptionSettings;

    public SubscriptionStatusEventHandler(ILogger<SubscriptionStatusEventHandler> logger, IApplicationDbContext context, IMessagePublisher messagePublisher, ISubscriptionManagementService subscriptionManagementService, IOptions<SubscriptionSettings> subscriptionSettings)
    {
        _logger = logger;
        _context = context;
        _messagePublisher = messagePublisher;
        _subscriptionManagementService = subscriptionManagementService;
        _subscriptionSettings = subscriptionSettings.Value;
    }

    public async Task Handle(SubscriptionStatusEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing subscription status event: {Action} for subscription {SubscriptionId} (Immediate: {SuspendLimitsImmediately})",
            notification.Action, notification.SubscriptionId, notification.SuspendLimitsImmediately);

        try
        {
            var subscription = await _context.Subscriptions.Include(s => s.Tenant).Include(s => s.Plan).FirstOrDefaultAsync(s => s.Id == notification.SubscriptionId, cancellationToken);

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

        _logger.LogInformation("Updated subscription {SubscriptionId} for action {Action}", subscription.Id, notification.Action);
    }

    private async Task HandleSubscriptionLimitsAsync(Subscription subscription, SubscriptionStatusEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Create a subscription message event to sync entities with plan limits
            var subscriptionEvent = new SubscriptionMessageEvent(notification.TenantId, notification.ApplicationUserId)
            {
                CorrelationId = notification.CorrelationId,
                ApplicationUserPublicId = notification.ApplicationUserPublicId,
            };

            var queue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Subscription);
            await _messagePublisher.PublishAsync(subscriptionEvent, queue.RoutingKey, cancellationToken);

            _logger.LogInformation("Successfully published subscription message event for tenant {TenantId} to sync entities", notification.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync entities with limits for action {Action} - Subscription {SubscriptionId}", notification.Action, subscription.Id);
            // Don't throw - this shouldn't fail the main subscription status update
        }
    }

    private async Task SendEmailNotificationAsync(Subscription subscription, SubscriptionStatusEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var templateId = GetEmailTemplateId(notification.Action);
            var subject = GetEmailSubject(notification.Action, notification.SuspendLimitsImmediately);

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
                    { "reason", notification.Reason },
                    { "subscriptionId", subscription.Id },
                    { "planName", subscription.Plan?.Name ?? "Current Plan" },
                    { "subscriptionAction", notification.Action.ToString() },
                    { "suspendedAt", DateTimeOffset.UtcNow.ToString("MMMM dd, yyyy") },
                    { "reactivatedAt", DateTimeOffset.UtcNow.ToString("MMMM dd, yyyy") },
                    { "cancelledAt", DateTimeOffset.UtcNow.ToString("MMMM dd, yyyy") },
                    { "gracePeriodDays", _subscriptionSettings?.GracePeriodDays.ToString() ?? "7" },
                    { "gracePeriodEndDate", subscription.GracePeriodEndsAt?.ToString("MMMM dd, yyyy") ?? string.Empty },
                    { "isImmediate", notification.SuspendLimitsImmediately.ToString() },
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

    private string GetEmailTemplateId(SubscriptionAction action) => action switch
    {
        SubscriptionAction.Create => EmailTemplates.SubscriptionCreated,
        SubscriptionAction.Suspend => EmailTemplates.SubscriptionSuspended,
        SubscriptionAction.Reactivate => EmailTemplates.SubscriptionReactivated,
        SubscriptionAction.Cancel => EmailTemplates.SubscriptionCancelled,
        SubscriptionAction.GracePeriodStart => EmailTemplates.SubscriptionGracePeriodStart,
        SubscriptionAction.GracePeriodEnd => EmailTemplates.SubscriptionGracePeriodEnd,
        SubscriptionAction.PlanChanged => EmailTemplates.SubscriptionPlanChanged,
        _ => EmailTemplates.SubscriptionSuspended
    };

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
}