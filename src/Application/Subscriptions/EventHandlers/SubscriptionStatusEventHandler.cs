using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Application.Common.Exceptions;

namespace ConnectFlow.Application.Subscriptions.EventHandlers;

/// <summary>
/// Handles subscription status changes - suspends/reactivates subscriptions, manages limits, and sends notifications
/// </summary>
public class SubscriptionStatusEventHandler : INotificationHandler<SubscriptionStatusEvent>
{
    private readonly ILogger<SubscriptionStatusEventHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IMessagePublisher _messagePublisher;
    private readonly SubscriptionSettings _subscriptionSettings;

    public SubscriptionStatusEventHandler(ILogger<SubscriptionStatusEventHandler> logger, IApplicationDbContext context, IMessagePublisher messagePublisher, IOptions<SubscriptionSettings> subscriptionSettings)
    {
        _logger = logger;
        _context = context;
        _messagePublisher = messagePublisher;
        _subscriptionSettings = subscriptionSettings.Value;
    }

    public async Task Handle(SubscriptionStatusEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing subscription status event: {Action} for subscription {SubscriptionId} (Immediate: {IsImmediate})", notification.Action, notification.Subscription.Id, notification.IsImmediate);

        try
        {
            var subscription = notification.Subscription;

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
                notification.Subscription.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process subscription status event for {SubscriptionId}",
                notification.Subscription.Id);
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
                if (notification.IsImmediate)
                {
                    subscription.Status = SubscriptionStatus.Canceled;

                    //Auto subscribe to free plan after cancelling current subscription
                    var freePlan = await _context.Plans.FirstOrDefaultAsync(p => p.Name.ToLower() == _subscriptionSettings.DefaultDowngradePlanName.ToLower() && p.IsActive, cancellationToken);
                    if (freePlan != null)
                    {
                        // Create new free subscription
                        var freeSubscription = new Subscription
                        {
                            PaymentProviderSubscriptionId = $"free_{Guid.NewGuid()}",
                            Status = SubscriptionStatus.Active,
                            CurrentPeriodStart = DateTimeOffset.UtcNow,
                            CurrentPeriodEnd = DateTimeOffset.UtcNow.AddYears(100), // Free plan never expires
                            CancelAtPeriodEnd = false,
                            PlanId = freePlan.Id,
                            Amount = freePlan.Price,
                            Currency = freePlan.Currency,
                            TenantId = subscription.TenantId
                        };

                        _context.Subscriptions.Add(freeSubscription);

                        // Add event for new free subscription creation
                        freeSubscription.AddDomainEvent(new SubscriptionStatusEvent(subscription.TenantId, notification.ApplicationUserId,
                            freeSubscription,
                            SubscriptionAction.Create,
                            $"Auto subscribed to free plan {freePlan.Name} after cancellation of {subscription.Plan.Name}",
                            sendEmailNotification: true));
                    }
                }

                break;

            case SubscriptionAction.GracePeriodStart:
                subscription.Status = SubscriptionStatus.PastDue;
                break;

            case SubscriptionAction.GracePeriodEnd:
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
            //check if subscription does not have tenant loaded then load now
            if (subscription.Tenant == null)
            {
                subscription.Tenant = await _context.Tenants.FindAsync(new object[] { subscription.TenantId }, cancellationToken) ?? throw new TenantNotFoundException($"Tenant not found for subscription {subscription.Id}");
            }

            var templateId = GetEmailTemplateId(notification.Action);
            var subject = GetEmailSubject(notification.Action, notification.IsImmediate);

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
                    { "tenantName", subscription.Tenant?.Name ?? "Valued Customer" },
                    { "reason", notification.Reason },
                    { "subscriptionId", subscription.Id },
                    { "planName", subscription.Plan?.Name ?? "Current Plan" },
                    { "subscriptionAction", notification.Action.ToString() },
                    { "suspendedAt", DateTimeOffset.UtcNow.ToString("MMMM dd, yyyy") },
                    { "reactivatedAt", DateTimeOffset.UtcNow.ToString("MMMM dd, yyyy") },
                    { "cancelledAt", DateTimeOffset.UtcNow.ToString("MMMM dd, yyyy") },
                    { "gracePeriodDays", _subscriptionSettings?.GracePeriodDays.ToString() ?? "7" },
                    { "gracePeriodEndDate", subscription.GracePeriodEndsAt?.ToString("MMMM dd, yyyy") ?? string.Empty },
                    { "isImmediate", notification.IsImmediate.ToString() },
                    { "correlationId", notification.CorrelationId.GetValueOrDefault() }
                },
            };

            var queue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Email);
            await _messagePublisher.PublishAsync(emailEvent, queue.RoutingKey, cancellationToken);

            _logger.LogInformation("Queued {Action} email notification for tenant {TenantEmail} for subscription {SubscriptionId}", notification.Action, subscription.Tenant?.Email, subscription.Id);
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

    private string GetEmailSubject(SubscriptionAction action, bool isImmediate = false) => action switch
    {
        SubscriptionAction.Create => "Welcome! Your subscription is now active",
        SubscriptionAction.Suspend => "Your subscription has been suspended",
        SubscriptionAction.Reactivate => "Your subscription has been reactivated",
        SubscriptionAction.Cancel when isImmediate => "Your subscription has been cancelled immediately",
        SubscriptionAction.Cancel => "Your subscription will be cancelled at period end",
        SubscriptionAction.GracePeriodStart => "Payment required - Grace period started",
        SubscriptionAction.GracePeriodEnd => "Grace period ended",
        SubscriptionAction.PlanChanged => "Your subscription plan has been updated",
        _ => "Subscription update"
    };
}