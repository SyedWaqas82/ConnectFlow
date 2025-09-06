using ConnectFlow.Application.Common.Models;
using ConnectFlow.Infrastructure.Common.Metrics;
using MediatR;
using Newtonsoft.Json.Linq;

namespace ConnectFlow.Infrastructure.Services.Payment;

/// <summary>
/// Service that handles individual webhook event types
/// </summary>
public class StripeWebhookEventHandlerService : IPaymentWebhookEventHandlerService
{
    private readonly IPaymentService _paymentService;
    private readonly IApplicationDbContext _context;
    private readonly SubscriptionSettings _subscriptionSettings;
    private readonly ILogger<StripeWebhookEventHandlerService> _logger;
    private readonly PaymentMetrics _paymentMetrics;

    public StripeWebhookEventHandlerService(IPaymentService paymentService, IApplicationDbContext context, IOptions<SubscriptionSettings> subscriptionSettings, ILogger<StripeWebhookEventHandlerService> logger, PaymentMetrics paymentMetrics)
    {
        _paymentService = paymentService;
        _context = context;
        _subscriptionSettings = subscriptionSettings.Value;
        _logger = logger;
        _paymentMetrics = paymentMetrics;
    }

    public async Task<bool> HandleSubscriptionCreatedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken)
    {
        try
        {
            if (!ValidateRequiredFields(paymentEvent, paymentEvent.SubscriptionId, paymentEvent.CustomerId))
                return false;

            // Get the full subscription details from Stripe
            var subscription = await _paymentService.GetSubscriptionAsync(paymentEvent.SubscriptionId, cancellationToken);

            // Extract price ID from subscription data
            var priceId = subscription.PriceId;
            if (string.IsNullOrEmpty(priceId))
            {
                _logger.LogWarning("Could not extract price ID from subscription data in webhook event {EventId}", paymentEvent.Id);
                return false;
            }

            var plan = await _context.Plans.FirstOrDefaultAsync(p => p.PaymentProviderPriceId == priceId && p.IsActive, cancellationToken);

            if (plan == null)
            {
                _logger.LogWarning("Plan not found for price ID {PriceId}", priceId);
                return false;
            }

            // Check if subscription already exists
            var existingSubscription = await FindSubscriptionByStripeIdAsync(paymentEvent.SubscriptionId, cancellationToken, includePlan: false, includeTenant: false);

            if (existingSubscription != null)
            {
                _logger.LogInformation("Subscription {SubscriptionId} already exists, updating status", paymentEvent.SubscriptionId);
                return await UpdateExistingSubscriptionFromWebhook(existingSubscription, subscription, plan, cancellationToken);
            }

            return await CreateNewSubscriptionAsync(subscription, plan, paymentEvent.TenantId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling subscription created webhook event {EventId}", paymentEvent.Id);
            return false;
        }
    }

    public async Task<bool> HandleSubscriptionUpdatedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken)
    {
        try
        {
            if (!ValidateRequiredFields(paymentEvent, paymentEvent.SubscriptionId))
                return false;

            var existingSubscription = await FindSubscriptionByStripeIdAsync(paymentEvent.SubscriptionId, cancellationToken);
            if (existingSubscription == null) return false;

            // Get updated subscription details from Stripe
            var subscription = await _paymentService.GetSubscriptionAsync(paymentEvent.SubscriptionId, cancellationToken);

            return await UpdateExistingSubscriptionFromWebhook(existingSubscription, subscription, existingSubscription.Plan, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling subscription updated webhook event {EventId}", paymentEvent.Id);
            return false;
        }
    }

    public async Task<bool> HandleSubscriptionDeletedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken)
    {
        try
        {
            if (!ValidateRequiredFields(paymentEvent, paymentEvent.SubscriptionId))
                return false;

            var existingSubscription = await FindSubscriptionByStripeIdAsync(paymentEvent.SubscriptionId, cancellationToken);
            if (existingSubscription == null) return false;

            var subscription = await _paymentService.GetSubscriptionAsync(paymentEvent.SubscriptionId, cancellationToken);

            return await ProcessSubscriptionDeletionAsync(existingSubscription, subscription, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling subscription deleted webhook event {EventId}", paymentEvent.Id);
            return false;
        }
    }

    public async Task<bool> HandlePaymentAsync(PaymentEventDto paymentEvent, PaymentInvoiceStatus invoiceStatus, CancellationToken cancellationToken)
    {
        try
        {
            var invoiceData = paymentEvent.Data ?? new Dictionary<string, object>();

            var amountDue = Convert.ToDecimal(invoiceData.GetValueOrDefault("amount_due")) / 100; // Convert from cents
            var amountPaid = Convert.ToDecimal(invoiceData.GetValueOrDefault("amount_paid")) / 100; // Convert from cents
            var currency = invoiceData.GetValueOrDefault("currency")?.ToString() ?? "usd";
            var status = invoiceData.GetValueOrDefault("status")?.ToString() ?? "unknown";
            var invoicePdf = invoiceData.GetValueOrDefault("invoice_pdf")?.ToString() ?? string.Empty;
            var invoiceHostedUrl = invoiceData.GetValueOrDefault("hosted_invoice_url")?.ToString() ?? string.Empty;
            var attemptCount = Convert.ToInt32(invoiceData.GetValueOrDefault("attempt_count"));

            var paymentTransitions = (invoiceData.TryGetValue("status_transitions", out var pt) && pt is JObject metadataJObject) ? metadataJObject.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string>() : new Dictionary<string, string>();
            var paidAt = paymentTransitions.TryGetValue("paid_at", out var paidAtStr) && long.TryParse(paidAtStr, out var paidAtUnix) ? DateTimeOffset.FromUnixTimeSeconds(paidAtUnix) : (DateTimeOffset?)null;

            // Extract failure information
            var failureCode = invoiceData.GetValueOrDefault("failure_code")?.ToString() ?? "unknown";
            var failureMessage = invoiceData.GetValueOrDefault("failure_message")?.ToString() ?? "Payment failed";

            if (!ValidateRequiredFields(paymentEvent, paymentEvent.ObjectId, paymentEvent.SubscriptionId))
                return false;

            var existingSubscription = await FindSubscriptionByStripeIdAsync(paymentEvent.SubscriptionId, cancellationToken);
            if (existingSubscription == null) return false;

            if (invoiceStatus == PaymentInvoiceStatus.Succeeded)
            {
                // Track payment success metric
                _paymentMetrics.PaymentSuccessful(amountPaid, currency);

                _logger.LogInformation("Payment succeeded for subscription {SubscriptionId}, amount {Amount} {Currency}", existingSubscription.Id, amountPaid, currency);
            }
            else if (invoiceStatus == PaymentInvoiceStatus.Failed)
            {
                // Track payment failure metric with specific failure reason
                _paymentMetrics.PaymentFailed(amountDue, failureCode, currency);

                // Check if max retries reached - this info is passed to PaymentStatusEvent
                var hasReachedMaxRetries = attemptCount >= _subscriptionSettings.MaxPaymentRetries;
                if (hasReachedMaxRetries && !existingSubscription.HasReachedMaxRetries)
                {
                    existingSubscription.HasReachedMaxRetries = true;
                }
                existingSubscription.LastModified = DateTimeOffset.UtcNow; //touch the subscription to trigger domain event

                var paymentDomainEvent = new PaymentStatusEvent(existingSubscription.TenantId, default, existingSubscription, PaymentAction.Failed, $"Payment attempt {attemptCount} failed", amount: amountDue, sendEmailNotification: true, failureCount: attemptCount);
                existingSubscription.AddDomainEvent(paymentDomainEvent);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Payment failed for subscription {SubscriptionId}, attempt {AttemptCount}", existingSubscription.Id, attemptCount);
            }
            else if (invoiceStatus == PaymentInvoiceStatus.Created)
            {
                _logger.LogInformation("Invoice {InvoiceId} created for subscription {SubscriptionId}", paymentEvent.ObjectId, existingSubscription.Id);
            }
            else if (invoiceStatus == PaymentInvoiceStatus.Finalized)
            {
                _logger.LogInformation("Payment succeeded for subscription {SubscriptionId}, amount {Amount} {Currency}", existingSubscription.Id, amountPaid, currency);
            }
            else if (invoiceStatus == PaymentInvoiceStatus.Paid)
            {
                var paymentDomainEvent = new PaymentStatusEvent(existingSubscription.TenantId, default, existingSubscription, PaymentAction.Success, invoiceStatus.ToString(), amount: amountPaid, sendEmailNotification: true);

                existingSubscription.LastModified = DateTimeOffset.UtcNow; //touch the subscription to trigger domain event
                existingSubscription.AddDomainEvent(paymentDomainEvent);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Invoice {InvoiceId} marked as paid for subscription {SubscriptionId}", paymentEvent.ObjectId, existingSubscription.Id);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling invoice payment {invoiceStatus.ToString().ToLower()} webhook event {paymentEvent.Id}");
            return false;
        }
    }

    public async Task<bool> HandleUnknownEventAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unhandled webhook event type {EventType} with ID {EventId}", paymentEvent.Type, paymentEvent.Id);
        return await Task.FromResult(false);
    }

    private bool ValidateRequiredFields(PaymentEventDto paymentEvent, params string?[] requiredFields)
    {
        var missingFields = requiredFields.Where(field => string.IsNullOrEmpty(field)).ToList();
        if (missingFields.Any())
        {
            _logger.LogWarning("Missing required fields in webhook event {EventId}", paymentEvent.Id);
            return false;
        }
        return true;
    }

    private async Task<Subscription?> FindSubscriptionByStripeIdAsync(string stripeSubscriptionId, CancellationToken cancellationToken, bool includePlan = true, bool includeTenant = true)
    {
        var query = _context.Subscriptions.AsQueryable();

        if (includePlan)
            query = query.Include(s => s.Plan);
        if (includeTenant)
            query = query.Include(s => s.Tenant);

        var subscription = await query.FirstOrDefaultAsync(s => s.PaymentProviderSubscriptionId == stripeSubscriptionId, cancellationToken);

        if (subscription == null)
        {
            _logger.LogWarning("Subscription {SubscriptionId} not found", stripeSubscriptionId);
        }

        return subscription;
    }

    private async Task<bool> CreateNewSubscriptionAsync(PaymentSubscriptionDto subscription, Plan plan, int tenantId, CancellationToken cancellationToken)
    {
        var newSubscription = new Subscription
        {
            PaymentProviderSubscriptionId = subscription.Id,
            Status = MapStripeStatusToSubscriptionStatus(subscription.Status),
            CurrentPeriodStart = subscription.CurrentPeriodStart,
            CurrentPeriodEnd = subscription.CurrentPeriodEnd,
            CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
            PlanId = plan.Id,
            Amount = plan.Price,
            Currency = plan.Currency,
            TenantId = tenantId
        };

        if (subscription.CanceledAt.HasValue)
        {
            newSubscription.CanceledAt = subscription.CanceledAt.Value;
        }

        // Add domain event for subscription creation
        // Also, raise plan change event so entities get synced to plan limits if coming from an old cancelled subscription
        newSubscription.AddDomainEvent(new SubscriptionStatusEvent(tenantId, default, newSubscription, SubscriptionAction.Create, "Subscription created via Stripe webhook", sendEmailNotification: true));
        newSubscription.AddDomainEvent(new SubscriptionStatusEvent(tenantId, default, newSubscription, SubscriptionAction.PlanChanged, "Subscription plan changed", sendEmailNotification: false));

        _context.Subscriptions.Add(newSubscription);

        await _context.SaveChangesAsync(cancellationToken);

        // Record metrics
        _paymentMetrics.SubscriptionCreated(plan.Type.ToString(), plan.Price);

        _logger.LogInformation("Created subscription {SubscriptionId} for tenant {TenantId}", newSubscription.Id, tenantId);

        return true;
    }

    private async Task<bool> UpdateExistingSubscriptionFromWebhook(Subscription existingSubscription, PaymentSubscriptionDto subscription, Plan currentPlan, CancellationToken cancellationToken)
    {
        var previousStatus = existingSubscription.Status;
        var previousCancelAtPeriodEnd = existingSubscription.CancelAtPeriodEnd;
        var newStatus = MapStripeStatusToSubscriptionStatus(subscription.Status);

        // Check for plan changes by comparing price IDs
        Plan? newPlan = null;
        var hasPlansChanged = false;

        if (!string.IsNullOrEmpty(subscription.PriceId) && subscription.PriceId != currentPlan.PaymentProviderPriceId)
        {
            // Find the new plan by price ID
            newPlan = await _context.Plans.FirstOrDefaultAsync(p => p.PaymentProviderPriceId == subscription.PriceId && p.IsActive, cancellationToken);

            if (newPlan != null)
            {
                hasPlansChanged = true;
                _logger.LogInformation("Plan change detected for subscription {SubscriptionId}: {OldPlan} -> {NewPlan}", existingSubscription.Id, currentPlan.Name, newPlan.Name);

                // Update the plan reference
                existingSubscription.PlanId = newPlan.Id;
                existingSubscription.Amount = newPlan.Price;
                existingSubscription.Currency = newPlan.Currency;
            }
            else
            {
                _logger.LogWarning("New plan not found for price ID {PriceId} in subscription update", subscription.PriceId);
                newPlan = currentPlan; // Fallback to current plan
            }
        }
        else
        {
            newPlan = currentPlan;
        }

        // Update subscription data from Stripe (keep data synchronization)
        // Note: Status changes will be handled by event handlers
        existingSubscription.CurrentPeriodStart = subscription.CurrentPeriodStart;
        existingSubscription.CurrentPeriodEnd = subscription.CurrentPeriodEnd;

        // Handle cancellation properties carefully
        existingSubscription.CancelAtPeriodEnd = subscription.CancelAtPeriodEnd;

        // Set CanceledAt when we receive it from Stripe, or when CancelAtPeriodEnd is set for the first time
        if (subscription.CanceledAt.HasValue)
        {
            existingSubscription.CanceledAt = subscription.CanceledAt.Value;
        }

        // Track when cancellation was originally requested
        if (subscription.CancelAtPeriodEnd && !previousCancelAtPeriodEnd)
        {
            // Cancellation was newly requested
            existingSubscription.CancellationRequestedAt = DateTimeOffset.UtcNow;

            // Don't set CanceledAt yet for period-end cancellations - that's when it actually gets canceled
            if (!subscription.CanceledAt.HasValue)
            {
                existingSubscription.CanceledAt = null;
            }
        }
        else if (!subscription.CancelAtPeriodEnd && previousCancelAtPeriodEnd)
        {
            // Cancellation was reversed, clear the request timestamp
            existingSubscription.CancellationRequestedAt = null;
            existingSubscription.CanceledAt = null;
        }

        // Add appropriate domain events before database save
        if (hasPlansChanged)
        {
            // Record plan change metrics
            _paymentMetrics.SubscriptionUpdated("plan_change", currentPlan.Type.ToString(), newPlan.Type.ToString());

            var planChangeEvent = new SubscriptionStatusEvent(existingSubscription.TenantId, default, existingSubscription, SubscriptionAction.PlanChanged, $"Plan changed from {currentPlan.Name} to {newPlan.Name}", sendEmailNotification: true);

            existingSubscription.AddDomainEvent(planChangeEvent);

            _logger.LogInformation("Subscription {SubscriptionId} plan changed from {FromPlan} to {ToPlan}", existingSubscription.Id, currentPlan.Name, newPlan.Name);
        }

        // Track if we've already handled cancellation to avoid duplicates
        var cancellationEventAdded = false;

        // Handle status changes with events
        if (previousStatus != newStatus)
        {
            var action = newStatus switch
            {
                SubscriptionStatus.Canceled => SubscriptionAction.Cancel,
                SubscriptionStatus.Active when previousStatus != SubscriptionStatus.Active => SubscriptionAction.Reactivate,
                _ => SubscriptionAction.StatusUpdate
            };

            if (action != SubscriptionAction.StatusUpdate)
            {
                // For actual cancellation (status = "canceled"), this is immediate regardless of the flag
                var isImmediateAction = action == SubscriptionAction.Cancel;

                var statusEvent = new SubscriptionStatusEvent(existingSubscription.TenantId, default, existingSubscription, action, $"Status changed from {previousStatus} to {newStatus}", sendEmailNotification: true, isImmediate: isImmediateAction);

                existingSubscription.AddDomainEvent(statusEvent);

                // Track if we added a cancellation event
                if (action == SubscriptionAction.Cancel)
                {
                    cancellationEventAdded = true;
                }

                _logger.LogInformation("Subscription {SubscriptionId} status changed from {PreviousStatus} to {NewStatus} - action: {Action}", existingSubscription.Id, previousStatus, newStatus, action);
            }
        }

        // Handle cancellation flag changes (but avoid duplicate cancellation events)
        if (previousCancelAtPeriodEnd != subscription.CancelAtPeriodEnd)
        {
            if (subscription.CancelAtPeriodEnd && !previousCancelAtPeriodEnd && !cancellationEventAdded)
            {
                var cancelEvent = new SubscriptionStatusEvent(existingSubscription.TenantId, default, existingSubscription, SubscriptionAction.Cancel, "Subscription set to cancel at period end", sendEmailNotification: true, isImmediate: false); // Period-end cancellation is not immediate

                existingSubscription.AddDomainEvent(cancelEvent);

                _logger.LogInformation("Subscription {SubscriptionId} set to cancel at period end ({PeriodEnd})", existingSubscription.Id, subscription.CurrentPeriodEnd);
            }
            else if (!subscription.CancelAtPeriodEnd && previousCancelAtPeriodEnd)
            {
                var reactivateEvent = new SubscriptionStatusEvent(existingSubscription.TenantId, default, existingSubscription, SubscriptionAction.Reactivate, "Cancellation reversed - subscription reactivated", sendEmailNotification: true);

                existingSubscription.AddDomainEvent(reactivateEvent);

                _logger.LogInformation("Subscription {SubscriptionId} cancellation was reversed - subscription reactivated", existingSubscription.Id);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated subscription {SubscriptionId} status from {PreviousStatus} to {NewStatus}", existingSubscription.Id, previousStatus, newStatus);
        return true;
    }

    private async Task<bool> ProcessSubscriptionDeletionAsync(Subscription existingSubscription, PaymentSubscriptionDto subscription, CancellationToken cancellationToken)
    {
        var wasScheduledForCancellation = existingSubscription.CancelAtPeriodEnd;

        // Track subscription cancellation metric
        var cancellationType = wasScheduledForCancellation ? "at_period_end" : "immediate";
        _paymentMetrics.SubscriptionCanceled(cancellationType, existingSubscription.Plan?.Type.ToString() ?? "Unknown");

        // Update cancellation timestamps (keep data synchronization)
        // Note: Status change will be handled by event handler
        existingSubscription.CanceledAt = subscription.CanceledAt ?? DateTimeOffset.UtcNow; // When it was actually canceled
        existingSubscription.CancelAtPeriodEnd = false; // Reset flag since cancellation is now complete

        _logger.LogInformation("Subscription {SubscriptionId} deleted - was scheduled for cancellation: {WasScheduled}", existingSubscription.Id, wasScheduledForCancellation);

        // Add cancellation domain event
        var cancelEvent = new SubscriptionStatusEvent(existingSubscription.TenantId, default, existingSubscription, SubscriptionAction.Cancel, wasScheduledForCancellation ? "Subscription reached period end and was canceled as scheduled" : "Subscription was canceled immediately", sendEmailNotification: true); // Always suspend limits immediately when subscription is actually deleted

        existingSubscription.AddDomainEvent(cancelEvent);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Processed subscription deletion for {SubscriptionId}, was scheduled: {WasScheduled}", existingSubscription.Id, wasScheduledForCancellation);
        return true;
    }

    private static SubscriptionStatus MapStripeStatusToSubscriptionStatus(string stripeStatus)
    {
        return stripeStatus?.ToLowerInvariant() switch
        {
            "active" => SubscriptionStatus.Active,
            "canceled" => SubscriptionStatus.Canceled,
            "past_due" => SubscriptionStatus.PastDue,
            "unpaid" => SubscriptionStatus.Unpaid,
            "trialing" => SubscriptionStatus.Trialing,
            "incomplete" => SubscriptionStatus.Incomplete,
            "incomplete_expired" => SubscriptionStatus.IncompleteExpired,
            _ => SubscriptionStatus.Active
        };
    }
}