using ConnectFlow.Application.Common.Models;

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

    public StripeWebhookEventHandlerService(IPaymentService paymentService, IApplicationDbContext context, IOptions<SubscriptionSettings> subscriptionSettings, ILogger<StripeWebhookEventHandlerService> logger)
    {
        _paymentService = paymentService;
        _context = context;
        _subscriptionSettings = subscriptionSettings.Value;
        _logger = logger;
    }

    public async Task<bool> HandleSubscriptionCreatedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken)
    {
        try
        {
            var subscriptionData = paymentEvent.Data as Dictionary<string, object> ?? new Dictionary<string, object>();
            var stripeSubscriptionId = subscriptionData.GetValueOrDefault("id")?.ToString();
            var stripeCustomerId = subscriptionData.GetValueOrDefault("customer")?.ToString();

            if (!ValidateRequiredFields(paymentEvent, stripeSubscriptionId, stripeCustomerId))
                return false;

            var tenant = await FindTenantByCustomerIdAsync(stripeCustomerId!, cancellationToken);
            if (tenant == null) return false;

            // Get the full subscription details from Stripe
            var subscription = await _paymentService.GetSubscriptionAsync(stripeSubscriptionId!, cancellationToken);

            // Extract price ID from subscription data
            var priceId = ExtractPriceIdFromSubscriptionData(subscriptionData);
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
            var existingSubscription = await FindSubscriptionByStripeIdAsync(stripeSubscriptionId!, cancellationToken, includePlan: false, includeTenant: false);

            if (existingSubscription != null)
            {
                _logger.LogInformation("Subscription {SubscriptionId} already exists, updating status", stripeSubscriptionId);
                return await UpdateExistingSubscriptionFromWebhook(existingSubscription, subscription, plan, cancellationToken);
            }

            return await CreateNewSubscriptionAsync(subscription, plan, tenant, cancellationToken);
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
            var subscriptionData = paymentEvent.Data as Dictionary<string, object> ?? new Dictionary<string, object>();
            var stripeSubscriptionId = subscriptionData.GetValueOrDefault("id")?.ToString();

            if (!ValidateRequiredFields(paymentEvent, stripeSubscriptionId))
                return false;

            var existingSubscription = await FindSubscriptionByStripeIdAsync(stripeSubscriptionId!, cancellationToken);
            if (existingSubscription == null) return false;

            // Get updated subscription details from Stripe
            var subscription = await _paymentService.GetSubscriptionAsync(stripeSubscriptionId!, cancellationToken);

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
            var subscriptionData = paymentEvent.Data as Dictionary<string, object> ?? new Dictionary<string, object>();
            var stripeSubscriptionId = subscriptionData.GetValueOrDefault("id")?.ToString();

            if (!ValidateRequiredFields(paymentEvent, stripeSubscriptionId))
                return false;

            var existingSubscription = await FindSubscriptionByStripeIdAsync(stripeSubscriptionId!, cancellationToken);
            if (existingSubscription == null) return false;

            return await ProcessSubscriptionDeletionAsync(existingSubscription, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling subscription deleted webhook event {EventId}", paymentEvent.Id);
            return false;
        }
    }

    public async Task<bool> HandleInvoicePaymentSucceededAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken)
    {
        try
        {
            var invoiceData = paymentEvent.Data as Dictionary<string, object> ?? new Dictionary<string, object>();
            var stripeInvoiceId = invoiceData.GetValueOrDefault("id")?.ToString();
            var stripeSubscriptionId = invoiceData.GetValueOrDefault("subscription")?.ToString();
            var amountPaid = Convert.ToDecimal(invoiceData.GetValueOrDefault("amount_paid")) / 100; // Convert from cents
            var currency = invoiceData.GetValueOrDefault("currency")?.ToString() ?? "usd";

            if (!ValidateRequiredFields(paymentEvent, stripeInvoiceId, stripeSubscriptionId))
                return false;

            var existingSubscription = await FindSubscriptionByStripeIdAsync(stripeSubscriptionId!, cancellationToken);
            if (existingSubscription == null) return false;

            return await ProcessPaymentSuccessAsync(existingSubscription, stripeInvoiceId!, amountPaid, currency, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling invoice payment succeeded webhook event {EventId}", paymentEvent.Id);
            return false;
        }
    }

    public async Task<bool> HandleInvoicePaymentFailedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken)
    {
        try
        {
            var invoiceData = paymentEvent.Data as Dictionary<string, object> ?? new Dictionary<string, object>();
            var stripeInvoiceId = invoiceData.GetValueOrDefault("id")?.ToString();
            var stripeSubscriptionId = invoiceData.GetValueOrDefault("subscription")?.ToString();
            var amountDue = Convert.ToDecimal(invoiceData.GetValueOrDefault("amount_due")) / 100; // Convert from cents
            var currency = invoiceData.GetValueOrDefault("currency")?.ToString() ?? "usd";
            var attemptCount = Convert.ToInt32(invoiceData.GetValueOrDefault("attempt_count"));

            if (!ValidateRequiredFields(paymentEvent, stripeInvoiceId, stripeSubscriptionId))
                return false;

            var existingSubscription = await FindSubscriptionByStripeIdAsync(stripeSubscriptionId!, cancellationToken);
            if (existingSubscription == null) return false;

            return await ProcessPaymentFailureAsync(existingSubscription, stripeInvoiceId!, amountDue, currency, attemptCount, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling invoice payment failed webhook event {EventId}", paymentEvent.Id);
            return false;
        }
    }

    public async Task<bool> HandleInvoiceFinalizedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken)
    {
        try
        {
            var invoiceData = paymentEvent.Data as Dictionary<string, object> ?? new Dictionary<string, object>();
            var stripeInvoiceId = invoiceData.GetValueOrDefault("id")?.ToString();
            var stripeSubscriptionId = invoiceData.GetValueOrDefault("subscription")?.ToString();
            var amountDue = Convert.ToDecimal(invoiceData.GetValueOrDefault("amount_due")) / 100;
            var currency = invoiceData.GetValueOrDefault("currency")?.ToString() ?? "usd";

            if (!ValidateRequiredFields(paymentEvent, stripeInvoiceId, stripeSubscriptionId))
                return false;

            var existingSubscription = await FindSubscriptionByStripeIdAsync(stripeSubscriptionId!, cancellationToken);
            if (existingSubscription == null) return false;

            // Create or update invoice record
            await CreateOrUpdateInvoiceAsync(stripeInvoiceId!, existingSubscription.Id, amountDue, currency, "open", cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Invoice {InvoiceId} finalized for subscription {SubscriptionId}", stripeInvoiceId, existingSubscription.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling invoice finalized webhook event {EventId}", paymentEvent.Id);
            return false;
        }
    }

    public async Task<bool> HandleCheckoutSessionCompletedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken)
    {
        try
        {
            var sessionData = paymentEvent.Data as Dictionary<string, object> ?? new Dictionary<string, object>();
            var sessionId = sessionData.GetValueOrDefault("id")?.ToString();
            var stripeCustomerId = sessionData.GetValueOrDefault("customer")?.ToString();
            var stripeSubscriptionId = sessionData.GetValueOrDefault("subscription")?.ToString();

            if (!ValidateRequiredFields(paymentEvent, sessionId, stripeCustomerId))
                return false;

            var tenant = await FindTenantByCustomerIdAsync(stripeCustomerId!, cancellationToken);
            if (tenant == null) return false;


            _logger.LogInformation("Checkout session {SessionId} completed for tenant {TenantId}, subscription {SubscriptionId}", sessionId, tenant.Id, stripeSubscriptionId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling checkout session completed webhook event {EventId}", paymentEvent.Id);
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

    private async Task<Tenant?> FindTenantByCustomerIdAsync(string stripeCustomerId, CancellationToken cancellationToken)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.PaymentProviderCustomerId == stripeCustomerId, cancellationToken);

        if (tenant == null)
        {
            _logger.LogWarning("Tenant not found for customer ID {CustomerId}", stripeCustomerId);
        }

        return tenant;
    }

    private string? ExtractPriceIdFromSubscriptionData(Dictionary<string, object> subscriptionData)
    {
        var priceId = subscriptionData.GetValueOrDefault("items")?.ToString();
        if (!string.IsNullOrEmpty(priceId)) return priceId;

        // Try to get from subscription items
        if (subscriptionData.TryGetValue("items", out var itemsObj) && itemsObj is Dictionary<string, object> items)
        {
            if (items.TryGetValue("data", out var dataObj) && dataObj is object[] dataArray && dataArray.Length > 0)
            {
                if (dataArray[0] is Dictionary<string, object> firstItem && firstItem.TryGetValue("price", out var priceObj))
                {
                    if (priceObj is Dictionary<string, object> price && price.TryGetValue("id", out var priceIdObj))
                    {
                        return priceIdObj?.ToString();
                    }
                }
            }
        }

        return null;
    }

    private async Task CreateOrUpdateInvoiceAsync(string stripeInvoiceId, int subscriptionId, decimal amount, string currency, string status, CancellationToken cancellationToken)
    {
        var existingInvoice = await _context.Invoices.FirstOrDefaultAsync(i => i.PaymentProviderInvoiceId == stripeInvoiceId, cancellationToken);

        if (existingInvoice == null)
        {
            var invoice = new Invoice
            {
                PaymentProviderInvoiceId = stripeInvoiceId,
                SubscriptionId = subscriptionId,
                Amount = amount,
                Currency = currency,
                Status = status
            };

            _context.Invoices.Add(invoice);
        }
        else
        {
            existingInvoice.Amount = amount;
            existingInvoice.Currency = currency;
            existingInvoice.Status = status;
        }
    }

    private async Task<bool> CreateNewSubscriptionAsync(PaymentSubscriptionDto subscription, Plan plan, Tenant tenant, CancellationToken cancellationToken)
    {
        var newSubscription = new Subscription
        {
            PaymentProviderSubscriptionId = subscription.Id,
            Status = MapStripeStatusToSubscriptionStatus(subscription.Status),
            CurrentPeriodStart = subscription.CurrentPeriodStart,
            CurrentPeriodEnd = subscription.CurrentPeriodEnd,
            CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
            PlanId = plan.Id,
            TenantId = tenant.Id
        };

        if (subscription.CanceledAt.HasValue)
        {
            newSubscription.CanceledAt = subscription.CanceledAt.Value;
        }

        // Add domain event for subscription creation
        var subscriptionEvent = new SubscriptionStatusEvent(tenant.Id, default, newSubscription.Id, SubscriptionAction.Create, "Subscription created via Stripe webhook", sendEmailNotification: true);

        newSubscription.AddDomainEvent(subscriptionEvent);

        _context.Subscriptions.Add(newSubscription);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created subscription {SubscriptionId} for tenant {TenantId}", newSubscription.Id, tenant.Id);
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
            existingSubscription.CancellationRequestedAt = subscription.CanceledAt ?? DateTimeOffset.UtcNow;

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
            var planChangeEvent = new SubscriptionStatusEvent(existingSubscription.TenantId, default, existingSubscription.Id, SubscriptionAction.PlanChanged,
                $"Plan changed from {currentPlan.Name} to {newPlan.Name}", sendEmailNotification: true, suspendLimitsImmediately: true, // Plan changes are immediate
                previousPlanId: currentPlan.Id, newPlanId: newPlan.Id);

            existingSubscription.AddDomainEvent(planChangeEvent);

            _logger.LogInformation("Subscription {SubscriptionId} plan changed from {FromPlan} to {ToPlan}",
                existingSubscription.Id, currentPlan.Name, newPlan.Name);
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
                var statusEvent = new SubscriptionStatusEvent(existingSubscription.TenantId, default, existingSubscription.Id, action, $"Status changed from {previousStatus} to {newStatus}",
                    sendEmailNotification: true, suspendLimitsImmediately: action == SubscriptionAction.Cancel); // Cancellations via status change are immediate

                existingSubscription.AddDomainEvent(statusEvent);

                // Track if we added a cancellation event
                if (action == SubscriptionAction.Cancel)
                {
                    cancellationEventAdded = true;
                }
            }
        }

        // Handle cancellation flag changes (but avoid duplicate cancellation events)
        if (previousCancelAtPeriodEnd != subscription.CancelAtPeriodEnd)
        {
            if (subscription.CancelAtPeriodEnd && !previousCancelAtPeriodEnd && !cancellationEventAdded)
            {
                var cancelEvent = new SubscriptionStatusEvent(existingSubscription.TenantId, default, existingSubscription.Id, SubscriptionAction.Cancel, "Subscription set to cancel at period end", sendEmailNotification: true, suspendLimitsImmediately: false); // Period-end cancellation is not immediate

                existingSubscription.AddDomainEvent(cancelEvent);

                _logger.LogInformation("Subscription {SubscriptionId} set to cancel at period end ({PeriodEnd})",
                    existingSubscription.Id, subscription.CurrentPeriodEnd);
            }
            else if (!subscription.CancelAtPeriodEnd && previousCancelAtPeriodEnd)
            {
                var reactivateEvent = new SubscriptionStatusEvent(existingSubscription.TenantId, default, existingSubscription.Id, SubscriptionAction.Reactivate, "Cancellation reversed - subscription reactivated", sendEmailNotification: true);

                existingSubscription.AddDomainEvent(reactivateEvent);

                _logger.LogInformation("Subscription {SubscriptionId} cancellation was reversed - subscription reactivated",
                    existingSubscription.Id);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated subscription {SubscriptionId} status from {PreviousStatus} to {NewStatus}", existingSubscription.Id, previousStatus, newStatus);
        return true;
    }

    private async Task<bool> ProcessSubscriptionDeletionAsync(Subscription existingSubscription, CancellationToken cancellationToken)
    {
        var wasScheduledForCancellation = existingSubscription.CancelAtPeriodEnd;

        // Update cancellation timestamps (keep data synchronization)
        // Note: Status change will be handled by event handler
        existingSubscription.CanceledAt = DateTimeOffset.UtcNow; // When it was actually canceled
        existingSubscription.CancelAtPeriodEnd = false; // Reset flag since cancellation is now complete

        _logger.LogInformation("Subscription {SubscriptionId} deleted - was scheduled for cancellation: {WasScheduled}",
            existingSubscription.Id, wasScheduledForCancellation);

        // Add cancellation domain event
        var cancelEvent = new SubscriptionStatusEvent(existingSubscription.TenantId, default, existingSubscription.Id, SubscriptionAction.Cancel, wasScheduledForCancellation ? "Subscription reached period end and was canceled as scheduled" : "Subscription was canceled immediately",
            sendEmailNotification: true, suspendLimitsImmediately: true); // Always suspend limits immediately when subscription is actually deleted

        existingSubscription.AddDomainEvent(cancelEvent);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Processed subscription deletion for {SubscriptionId}, was scheduled: {WasScheduled}",
            existingSubscription.Id, wasScheduledForCancellation);
        return true;
    }

    private async Task<bool> ProcessPaymentSuccessAsync(Subscription existingSubscription, string stripeInvoiceId, decimal amountPaid, string currency, CancellationToken cancellationToken)
    {
        // Create or update invoice record
        await CreateOrUpdateInvoiceAsync(stripeInvoiceId, existingSubscription.Id, amountPaid, currency, "paid", cancellationToken);

        // Add PaymentStatusEvent domain event for payment success
        // The PaymentStatusEventHandler will handle status updates and retry tracking reset
        var paymentEvent = new PaymentStatusEvent(existingSubscription.TenantId, default, existingSubscription.Id, PaymentAction.Success, "Payment succeeded", amount: amountPaid, sendEmailNotification: true);

        existingSubscription.AddDomainEvent(paymentEvent);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment succeeded for subscription {SubscriptionId}, amount {Amount} {Currency}",
            existingSubscription.Id, amountPaid, currency);
        return true;
    }

    private async Task<bool> ProcessPaymentFailureAsync(Subscription existingSubscription, string stripeInvoiceId, decimal amountDue, string currency, int attemptCount, CancellationToken cancellationToken)
    {
        // Create or update invoice record
        await CreateOrUpdateInvoiceAsync(stripeInvoiceId, existingSubscription.Id, amountDue, currency, "payment_failed", cancellationToken);

        // Check if max retries reached - this info is passed to PaymentStatusEvent
        var hasReachedMaxRetries = attemptCount >= _subscriptionSettings.MaxPaymentRetries;
        if (hasReachedMaxRetries && !existingSubscription.HasReachedMaxRetries)
        {
            existingSubscription.HasReachedMaxRetries = true;
        }

        // Add PaymentStatusEvent domain event for payment failure
        // The PaymentStatusEventHandler will handle suspension logic when SuspendOnFailure=true
        // The PaymentStatusEventHandler will handle retry tracking and grace period logic
        var paymentEvent = new PaymentStatusEvent(existingSubscription.TenantId, default, existingSubscription.Id, PaymentAction.Failed, $"Payment attempt {attemptCount} failed", amount: amountDue, sendEmailNotification: true, failureCount: attemptCount);

        existingSubscription.AddDomainEvent(paymentEvent);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment failed for subscription {SubscriptionId}, attempt {AttemptCount}", existingSubscription.Id, attemptCount);
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