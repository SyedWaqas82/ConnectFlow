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

            var plan = await FindPlanByPriceIdAsync(priceId, cancellationToken);
            if (plan == null) return false;

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

    private async Task<Plan?> FindPlanByPriceIdAsync(string priceId, CancellationToken cancellationToken)
    {
        var plan = await _context.Plans.FirstOrDefaultAsync(p => p.PaymentProviderPriceId == priceId && p.IsActive, cancellationToken);

        if (plan == null)
        {
            _logger.LogWarning("Plan not found for price ID {PriceId}", priceId);
        }

        return plan;
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

        _context.Subscriptions.Add(newSubscription);
        newSubscription.AddDomainEvent(new SubscriptionCreatedEvent(tenant.Id, newSubscription.Id, plan.Name));

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

        existingSubscription.Status = newStatus;
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

        // Handle plan changes (upgrade/downgrade)
        if (hasPlansChanged)
        {
            var isUpgrade = newPlan.Price > currentPlan.Price;
            var isDowngrade = newPlan.Price < currentPlan.Price;

            if (isUpgrade)
            {
                existingSubscription.AddDomainEvent(new SubscriptionUpgradedEvent(
                    existingSubscription.TenantId,
                    existingSubscription.Id,
                    currentPlan.Name,
                    newPlan.Name));

                _logger.LogInformation("Subscription {SubscriptionId} upgraded from {FromPlan} to {ToPlan}",
                    existingSubscription.Id, currentPlan.Name, newPlan.Name);
            }
            else if (isDowngrade)
            {
                existingSubscription.AddDomainEvent(new SubscriptionDowngradedEvent(
                    existingSubscription.TenantId,
                    existingSubscription.Id,
                    currentPlan.Name,
                    newPlan.Name));

                _logger.LogInformation("Subscription {SubscriptionId} downgraded from {FromPlan} to {ToPlan}",
                    existingSubscription.Id, currentPlan.Name, newPlan.Name);
            }
        }

        // Handle status changes
        if (previousStatus != newStatus)
        {
            existingSubscription.AddDomainEvent(new SubscriptionStatusUpdatedEvent(
                existingSubscription.TenantId,
                existingSubscription.Id,
                newPlan.Name,
                previousStatus,
                newStatus));

            // Handle cancellation scenarios
            if (newStatus == SubscriptionStatus.Canceled)
            {
                // Check if this was a scheduled cancellation at period end
                var wasScheduledForCancellation = existingSubscription.CancelAtPeriodEnd;

                if (wasScheduledForCancellation)
                {
                    _logger.LogInformation("Subscription {SubscriptionId} reached period end and was canceled as scheduled", existingSubscription.Id);

                    existingSubscription.AddDomainEvent(new SubscriptionCancelledEvent(
                        existingSubscription.TenantId,
                        existingSubscription.Id,
                        false)); // false = was scheduled for end of period
                }
                else
                {
                    _logger.LogInformation("Subscription {SubscriptionId} was canceled immediately", existingSubscription.Id);

                    existingSubscription.AddDomainEvent(new SubscriptionCancelledEvent(
                        existingSubscription.TenantId,
                        existingSubscription.Id,
                        true)); // true = immediate cancellation
                }

                // Handle auto-downgrade after grace period or cancellation
                if (existingSubscription.IsInGracePeriod)
                {
                    existingSubscription.IsInGracePeriod = false;
                    existingSubscription.GracePeriodEndsAt = null;

                    existingSubscription.AddDomainEvent(new SubscriptionGracePeriodEndedEvent(
                        existingSubscription.TenantId,
                        existingSubscription.Id,
                        newPlan.Name,
                        _subscriptionSettings.AutoDowngradeAfterGracePeriod));
                }

                // Auto-downgrade if configured
                if (_subscriptionSettings.AutoDowngradeAfterGracePeriod)
                {
                    await HandleAutoDowngradeToFreeAsync(existingSubscription, cancellationToken);
                }
            }
        }

        // Handle changes to cancel at period end flag
        if (previousCancelAtPeriodEnd != subscription.CancelAtPeriodEnd)
        {
            if (subscription.CancelAtPeriodEnd && !previousCancelAtPeriodEnd)
            {
                // Subscription was set to cancel at period end
                _logger.LogInformation("Subscription {SubscriptionId} set to cancel at period end ({PeriodEnd})",
                    existingSubscription.Id, subscription.CurrentPeriodEnd);

                existingSubscription.AddDomainEvent(new SubscriptionCancelledEvent(
                    existingSubscription.TenantId,
                    existingSubscription.Id,
                    false)); // false = scheduled for end of period
            }
            else if (!subscription.CancelAtPeriodEnd && previousCancelAtPeriodEnd)
            {
                // Cancellation was cancelled - subscription reactivated
                _logger.LogInformation("Subscription {SubscriptionId} cancellation was reversed - subscription reactivated",
                    existingSubscription.Id);

                existingSubscription.AddDomainEvent(new SubscriptionReactivatedEvent(
                    existingSubscription.TenantId,
                    existingSubscription.Id,
                    newPlan.Name));
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated subscription {SubscriptionId} status from {PreviousStatus} to {NewStatus}", existingSubscription.Id, previousStatus, newStatus);
        return true;
    }

    private async Task<bool> ProcessSubscriptionDeletionAsync(Subscription existingSubscription, CancellationToken cancellationToken)
    {
        var previousStatus = existingSubscription.Status;
        var wasScheduledForCancellation = existingSubscription.CancelAtPeriodEnd;

        // When subscription.deleted event is received, the subscription is definitely canceled
        existingSubscription.Status = SubscriptionStatus.Canceled;
        existingSubscription.CanceledAt = DateTimeOffset.UtcNow; // When it was actually canceled
        existingSubscription.CancelAtPeriodEnd = false; // Reset flag since cancellation is now complete

        _logger.LogInformation("Subscription {SubscriptionId} deleted - was scheduled for cancellation: {WasScheduled}",
            existingSubscription.Id, wasScheduledForCancellation);

        // Raise appropriate events based on whether this was scheduled or immediate
        existingSubscription.AddDomainEvent(new SubscriptionStatusUpdatedEvent(
            existingSubscription.TenantId,
            existingSubscription.Id,
            existingSubscription.Plan.Name,
            previousStatus,
            SubscriptionStatus.Canceled));

        if (wasScheduledForCancellation)
        {
            // This was a scheduled cancellation that reached its period end
            existingSubscription.AddDomainEvent(new SubscriptionCancelledEvent(
                existingSubscription.TenantId,
                existingSubscription.Id,
                false)); // false = was scheduled cancellation reaching period end

            _logger.LogInformation("Subscription {SubscriptionId} reached period end and completed scheduled cancellation",
                existingSubscription.Id);
        }
        else
        {
            // This was an immediate cancellation
            existingSubscription.AddDomainEvent(new SubscriptionCancelledEvent(
                existingSubscription.TenantId,
                existingSubscription.Id,
                true)); // true = immediate cancellation

            _logger.LogInformation("Subscription {SubscriptionId} was canceled immediately",
                existingSubscription.Id);
        }

        // Handle auto-downgrade if configured
        if (_subscriptionSettings.AutoDowngradeAfterGracePeriod)
        {
            await HandleAutoDowngradeToFreeAsync(existingSubscription, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Processed subscription deletion for {SubscriptionId}, was scheduled: {WasScheduled}",
            existingSubscription.Id, wasScheduledForCancellation);
        return true;
    }

    private async Task<bool> ProcessPaymentSuccessAsync(Subscription existingSubscription, string stripeInvoiceId, decimal amountPaid, string currency, CancellationToken cancellationToken)
    {
        // Update subscription status if it was in grace period or past due
        var previousStatus = existingSubscription.Status;
        if (existingSubscription.Status == SubscriptionStatus.PastDue || existingSubscription.IsInGracePeriod)
        {
            existingSubscription.Status = SubscriptionStatus.Active;
            existingSubscription.IsInGracePeriod = false;
            existingSubscription.GracePeriodEndsAt = null;
            existingSubscription.LastPaymentFailedAt = null;

            // Reset retry counters on successful payment
            ResetRetryTracking(existingSubscription);

            existingSubscription.AddDomainEvent(new SubscriptionStatusUpdatedEvent(
                existingSubscription.TenantId,
                existingSubscription.Id,
                existingSubscription.Plan.Name,
                previousStatus,
                SubscriptionStatus.Active));

            if (previousStatus == SubscriptionStatus.PastDue)
            {
                existingSubscription.AddDomainEvent(new SubscriptionGracePeriodEndedEvent(
                    existingSubscription.TenantId,
                    existingSubscription.Id,
                    existingSubscription.Plan.Name,
                    false));
            }
        }

        // Create or update invoice record
        await CreateOrUpdateInvoiceAsync(stripeInvoiceId, existingSubscription.Id, amountPaid, currency, "paid", cancellationToken);

        existingSubscription.AddDomainEvent(new PaymentSucceededEvent(
            existingSubscription.TenantId,
            existingSubscription.Id,
            stripeInvoiceId,
            amountPaid,
            currency));

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment succeeded for subscription {SubscriptionId}, amount {Amount} {Currency}",
            existingSubscription.Id, amountPaid, currency);
        return true;
    }

    private void ResetRetryTracking(Subscription subscription)
    {
        subscription.PaymentRetryCount = 0;
        subscription.FirstPaymentFailureAt = null;
        subscription.NextRetryAt = null;
        subscription.HasReachedMaxRetries = false;
    }

    private async Task<bool> ProcessPaymentFailureAsync(Subscription existingSubscription, string stripeInvoiceId, decimal amountDue, string currency, int attemptCount, CancellationToken cancellationToken)
    {
        var previousStatus = existingSubscription.Status;

        // Update retry tracking
        UpdateRetryTracking(existingSubscription, attemptCount);

        // Check if max retries reached
        var hasReachedMaxRetries = attemptCount >= _subscriptionSettings.MaxPaymentRetries;
        if (hasReachedMaxRetries && !existingSubscription.HasReachedMaxRetries)
        {
            existingSubscription.HasReachedMaxRetries = true;
            existingSubscription.AddDomainEvent(new SubscriptionMaxRetriesReachedEvent(
                existingSubscription.TenantId,
                existingSubscription.Id,
                existingSubscription.Plan.Name,
                attemptCount,
                existingSubscription.FirstPaymentFailureAt!.Value));
        }

        // Handle grace period logic
        await HandleGracePeriodLogicAsync(existingSubscription, attemptCount);

        // Handle auto-downgrade after max retries if enabled
        if (hasReachedMaxRetries && _subscriptionSettings.AutoDowngradeAfterMaxRetries)
        {
            await HandleAutoDowngradeAfterMaxRetriesAsync(existingSubscription, cancellationToken);
        }

        // Create or update invoice record
        await CreateOrUpdateInvoiceAsync(stripeInvoiceId, existingSubscription.Id, amountDue, currency, "payment_failed", cancellationToken);

        // Raise payment failed event
        existingSubscription.AddDomainEvent(new PaymentFailedEvent(
            existingSubscription.TenantId,
            existingSubscription.Id,
            stripeInvoiceId,
            amountDue,
            currency,
            $"Payment attempt {attemptCount} failed"));

        // Raise status updated event if status changed
        if (previousStatus != existingSubscription.Status)
        {
            existingSubscription.AddDomainEvent(new SubscriptionStatusUpdatedEvent(
                existingSubscription.TenantId,
                existingSubscription.Id,
                existingSubscription.Plan.Name,
                previousStatus,
                existingSubscription.Status));
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment failed for subscription {SubscriptionId}, entering grace period until {GracePeriodEnd}",
            existingSubscription.Id, existingSubscription.GracePeriodEndsAt);
        return true;
    }

    private void UpdateRetryTracking(Subscription subscription, int attemptCount)
    {
        subscription.LastPaymentFailedAt = DateTimeOffset.UtcNow;
        subscription.PaymentRetryCount = attemptCount;

        // Track first failure
        if (subscription.FirstPaymentFailureAt == null)
        {
            subscription.FirstPaymentFailureAt = DateTimeOffset.UtcNow;
        }

        // Calculate next retry time based on Stripe's retry schedule
        subscription.NextRetryAt = CalculateNextRetryTime(attemptCount, subscription.FirstPaymentFailureAt.Value);
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

    private async Task HandleGracePeriodLogicAsync(Subscription subscription, int attemptCount)
    {
        if (!subscription.IsInGracePeriod)
        {
            subscription.IsInGracePeriod = true;

            // Calculate grace period end date intelligently
            if (_subscriptionSettings.UseIntelligentGracePeriod)
            {
                subscription.GracePeriodEndsAt = CalculateIntelligentGracePeriodEnd(attemptCount, subscription.FirstPaymentFailureAt!.Value);
            }
            else
            {
                subscription.GracePeriodEndsAt = DateTimeOffset.UtcNow.AddDays(_subscriptionSettings.GracePeriodDays);
            }

            subscription.Status = SubscriptionStatus.PastDue;

            subscription.AddDomainEvent(new SubscriptionGracePeriodStartedEvent(
                subscription.TenantId,
                subscription.Id,
                subscription.Plan.Name,
                subscription.GracePeriodEndsAt.Value));

            subscription.AddDomainEvent(new SubscriptionPastDueEvent(
                subscription.TenantId,
                subscription.Id,
                subscription.Plan.Name,
                DateTimeOffset.UtcNow));
        }

        await Task.CompletedTask;
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

    private async Task HandleAutoDowngradeAfterMaxRetriesAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        try
        {
            // Find the free plan
            var freePlan = await _context.Plans.FirstOrDefaultAsync(p => p.Name.ToLower() == _subscriptionSettings.DefaultDowngradePlanName.ToLower() && p.IsActive, cancellationToken);

            if (freePlan == null)
            {
                _logger.LogWarning("Free plan '{PlanName}' not found for auto-downgrade after max retries", _subscriptionSettings.DefaultDowngradePlanName);
                return;
            }

            var currentPlan = subscription.Plan;

            // Create new free subscription immediately
            var freeSubscription = new Subscription
            {
                PaymentProviderSubscriptionId = $"free_{Guid.NewGuid()}",
                Status = SubscriptionStatus.Active,
                CurrentPeriodStart = DateTimeOffset.UtcNow,
                CurrentPeriodEnd = DateTimeOffset.UtcNow.AddYears(100), // Free plan never expires
                CancelAtPeriodEnd = false,
                PlanId = freePlan.Id,
                TenantId = subscription.TenantId
            };

            _context.Subscriptions.Add(freeSubscription);

            // Mark current subscription as canceled
            subscription.Status = SubscriptionStatus.Canceled;
            subscription.CanceledAt = DateTimeOffset.UtcNow;
            subscription.IsInGracePeriod = false;

            freeSubscription.AddDomainEvent(new SubscriptionDowngradedEvent(
                subscription.TenantId,
                subscription.Id,
                currentPlan.Name,
                freePlan.Name));

            _logger.LogInformation("Auto-downgraded subscription {SubscriptionId} to free plan after max retries for tenant {TenantId}",
                subscription.Id, subscription.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during auto-downgrade after max retries for subscription {SubscriptionId}", subscription.Id);
        }
    }

    private async Task HandleAutoDowngradeToFreeAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        try
        {
            // Find the free plan
            var freePlan = await _context.Plans.FirstOrDefaultAsync(p => p.Name.ToLower() == _subscriptionSettings.DefaultDowngradePlanName.ToLower() && p.IsActive, cancellationToken);

            if (freePlan == null)
            {
                _logger.LogWarning("Free plan '{PlanName}' not found for auto-downgrade", _subscriptionSettings.DefaultDowngradePlanName);
                return;
            }

            var currentPlan = subscription.Plan;

            // Create new free subscription
            var freeSubscription = new Subscription
            {
                PaymentProviderSubscriptionId = $"free_{Guid.NewGuid()}",
                Status = SubscriptionStatus.Active,
                CurrentPeriodStart = DateTimeOffset.UtcNow,
                CurrentPeriodEnd = DateTimeOffset.UtcNow.AddYears(100), // Free plan never expires
                CancelAtPeriodEnd = false,
                PlanId = freePlan.Id,
                TenantId = subscription.TenantId
            };

            _context.Subscriptions.Add(freeSubscription);

            freeSubscription.AddDomainEvent(new SubscriptionDowngradedEvent(subscription.TenantId, subscription.Id, currentPlan.Name, freePlan.Name));

            _logger.LogInformation("Auto-downgraded subscription {SubscriptionId} to free plan for tenant {TenantId}", subscription.Id, subscription.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during auto-downgrade to free plan for subscription {SubscriptionId}", subscription.Id);
        }
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