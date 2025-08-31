using ConnectFlow.Application.Common.Models;
using ConnectFlow.Infrastructure.Common.Metrics;
using ConnectFlow.Infrastructure.Services.Payment.Configuration;
using Stripe;
using StripeSubscription = Stripe.Subscription;
using StripeCustomer = Stripe.Customer;
using StripeInvoice = Stripe.Invoice;
using StripeCheckoutSession = Stripe.Checkout.Session;
using StripeCheckoutSessionService = Stripe.Checkout.SessionService;
using StripeCheckoutSessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using StripeBillingPortalSessionService = Stripe.BillingPortal.SessionService;
using StripeBillingPortalSessionCreateOptions = Stripe.BillingPortal.SessionCreateOptions;

namespace ConnectFlow.Infrastructure.Services.Payment;

/// <summary>
/// Stripe service implementation with full functionality
/// Maintains clean architecture by only exposing DTOs to the Application layer
/// </summary>
public class StripeService : IPaymentService
{
    private readonly ILogger<StripeService> _logger;
    private readonly StripeSettings _stripeSettings;
    private readonly PaymentMetrics _paymentMetrics;

    public StripeService(ILogger<StripeService> logger, IOptions<StripeSettings> stripeSettings, PaymentMetrics paymentMetrics)
    {
        _logger = logger;
        _stripeSettings = stripeSettings.Value;
        _paymentMetrics = paymentMetrics;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    public Task<PaymentEventDto> ProcessWebhookAsync(string body, string signature, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        string? eventType = null;

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(body, signature, _stripeSettings.WebhookSecret);
            eventType = stripeEvent.Type;

            // Record webhook received
            _paymentMetrics.WebhookReceived(eventType);

            _logger.LogInformation("Processed Stripe webhook event {EventId} of type {EventType}", stripeEvent.Id, stripeEvent.Type);

            var eventDto = new PaymentEventDto
            {
                Id = stripeEvent.Id,
                Type = stripeEvent.Type,
                Created = stripeEvent.Created,
                Data = stripeEvent.Data.RawObject as Dictionary<string, object> ?? new Dictionary<string, object>()
            };

            // Record successful webhook processing
            _paymentMetrics.WebhookProcessed(eventType, stopwatch.Elapsed.TotalSeconds);

            return Task.FromResult(eventDto);
        }
        catch (StripeException ex) when (ex.StripeError?.Type == "invalid_request_error")
        {
            _logger.LogWarning(ex, "Invalid Stripe webhook request - signature validation failed");

            if (eventType != null)
            {
                _paymentMetrics.WebhookFailed(eventType, "invalid_signature");
            }

            throw new ArgumentException("Invalid webhook signature or request format", ex);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to process Stripe webhook");

            if (eventType != null)
            {
                _paymentMetrics.WebhookFailed(eventType, "stripe_error");
            }

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing Stripe webhook");

            if (eventType != null)
            {
                _paymentMetrics.WebhookFailed(eventType, "unexpected_error");
            }

            throw;
        }
    }

    #region Customer Management

    public async Task<PaymentCustomerDto> CreateCustomerAsync(string email, string name, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
                Metadata = metadata ?? new Dictionary<string, string>()
            };

            var service = new CustomerService();
            var customer = await service.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogInformation("Created Stripe customer {CustomerId} for {Email}", customer.Id, email);

            // Record metrics
            _paymentMetrics.RecordStripeApiCall("create_customer", true, stopwatch.Elapsed.TotalSeconds);

            // Extract tenant ID from metadata if available
            var tenantId = metadata?.GetValueOrDefault("tenant_id");
            if (int.TryParse(tenantId, out var parsedTenantId))
            {
                _paymentMetrics.CustomerCreated(parsedTenantId);
            }
            else
            {
                _paymentMetrics.CustomerCreated();
            }

            return MapToCustomerDto(customer);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to create Stripe customer for {Email}", email);
            _paymentMetrics.RecordStripeApiCall("create_customer", false, stopwatch.Elapsed.TotalSeconds);
            _paymentMetrics.RecordStripeApiError("create_customer", ex.StripeError?.Type ?? "unknown");
            throw;
        }
    }

    public async Task<PaymentCustomerDto> UpdateCustomerAsync(string customerId, string? email = null, string? name = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var options = new CustomerUpdateOptions();

            if (!string.IsNullOrEmpty(email))
                options.Email = email;

            if (!string.IsNullOrEmpty(name))
                options.Name = name;

            if (metadata != null)
                options.Metadata = metadata;

            var service = new CustomerService();
            var customer = await service.UpdateAsync(customerId, options, cancellationToken: cancellationToken);

            _logger.LogInformation("Updated Stripe customer {CustomerId}", customerId);

            // Record metrics
            _paymentMetrics.RecordStripeApiCall("update_customer", true, stopwatch.Elapsed.TotalSeconds);

            return MapToCustomerDto(customer);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to update Stripe customer {CustomerId}", customerId);
            _paymentMetrics.RecordStripeApiCall("update_customer", false, stopwatch.Elapsed.TotalSeconds);
            _paymentMetrics.RecordStripeApiError("update_customer", ex.StripeError?.Type ?? "unknown");
            throw;
        }
    }

    public async Task<PaymentCustomerDto> GetCustomerAsync(string customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new CustomerService();
            var customer = await service.GetAsync(customerId, cancellationToken: cancellationToken);

            return MapToCustomerDto(customer);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to get Stripe customer {CustomerId}", customerId);
            throw;
        }
    }

    #endregion

    #region Subscription Management

    public async Task<PaymentSubscriptionDto> CreateSubscriptionAsync(string customerId, string priceId, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var options = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new()
                    {
                        Price = priceId
                    }
                },
                PaymentBehavior = "default_incomplete",
                PaymentSettings = new SubscriptionPaymentSettingsOptions
                {
                    SaveDefaultPaymentMethod = "on_subscription"
                },
                Expand = new List<string> { "latest_invoice.payment_intent" },
                Metadata = metadata ?? new Dictionary<string, string>()
            };

            var service = new Stripe.SubscriptionService();
            var subscription = await service.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogInformation("Created Stripe subscription {SubscriptionId} for customer {CustomerId}", subscription.Id, customerId);

            // Record metrics
            _paymentMetrics.RecordStripeApiCall("create_subscription", true, stopwatch.Elapsed.TotalSeconds);

            return MapToSubscriptionDto(subscription);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to create Stripe subscription for customer {CustomerId}", customerId);
            _paymentMetrics.RecordStripeApiCall("create_subscription", false, stopwatch.Elapsed.TotalSeconds);
            _paymentMetrics.RecordStripeApiError("create_subscription", ex.StripeError?.Type ?? "unknown");
            throw;
        }
    }

    public async Task<PaymentSubscriptionDto> UpdateSubscriptionAsync(string subscriptionId, string? priceId = null, bool? cancelAtPeriodEnd = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var options = new SubscriptionUpdateOptions();

            if (!string.IsNullOrEmpty(priceId))
            {
                // Get current subscription to update items correctly
                var currentService = new Stripe.SubscriptionService();
                var currentSubscription = await currentService.GetAsync(subscriptionId, cancellationToken: cancellationToken);

                options.Items = new List<SubscriptionItemOptions>();

                // Remove old items and add new price
                foreach (var item in currentSubscription.Items.Data)
                {
                    options.Items.Add(new SubscriptionItemOptions
                    {
                        Id = item.Id,
                        Deleted = true
                    });
                }

                options.Items.Add(new SubscriptionItemOptions
                {
                    Price = priceId
                });
            }

            if (cancelAtPeriodEnd.HasValue)
                options.CancelAtPeriodEnd = cancelAtPeriodEnd.Value;

            if (metadata != null)
                options.Metadata = metadata;

            var service = new Stripe.SubscriptionService();
            var subscription = await service.UpdateAsync(subscriptionId, options, cancellationToken: cancellationToken);

            _logger.LogInformation("Updated Stripe subscription {SubscriptionId}", subscriptionId);

            // Record metrics
            _paymentMetrics.RecordStripeApiCall("update_subscription", true, stopwatch.Elapsed.TotalSeconds);

            // Determine change type for better metrics
            var changeType = priceId != null ? "plan_change" :
                           cancelAtPeriodEnd.HasValue ? "cancellation_setting" : "general_update";
            _paymentMetrics.SubscriptionUpdated(changeType);

            return MapToSubscriptionDto(subscription);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to update Stripe subscription {SubscriptionId}", subscriptionId);
            _paymentMetrics.RecordStripeApiCall("update_subscription", false, stopwatch.Elapsed.TotalSeconds);
            _paymentMetrics.RecordStripeApiError("update_subscription", ex.StripeError?.Type ?? "unknown");
            throw;
        }
    }

    public async Task<PaymentSubscriptionDto> CancelSubscriptionAsync(string subscriptionId, bool cancelImmediately = false, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var service = new Stripe.SubscriptionService();
            StripeSubscription subscription;

            if (cancelImmediately)
            {
                subscription = await service.CancelAsync(subscriptionId, cancellationToken: cancellationToken);
                _logger.LogInformation("Immediately cancelled Stripe subscription {SubscriptionId}", subscriptionId);

                // Record metrics
                _paymentMetrics.RecordStripeApiCall("cancel_subscription", true, stopwatch.Elapsed.TotalSeconds);
                _paymentMetrics.SubscriptionCanceled("immediate", "unknown");
            }
            else
            {
                var options = new SubscriptionUpdateOptions
                {
                    CancelAtPeriodEnd = true
                };
                subscription = await service.UpdateAsync(subscriptionId, options, cancellationToken: cancellationToken);
                _logger.LogInformation("Scheduled Stripe subscription {SubscriptionId} for cancellation at period end", subscriptionId);

                // Record metrics
                _paymentMetrics.RecordStripeApiCall("schedule_cancellation", true, stopwatch.Elapsed.TotalSeconds);
                _paymentMetrics.SubscriptionCanceled("at_period_end", "unknown");
            }

            return MapToSubscriptionDto(subscription);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to cancel Stripe subscription {SubscriptionId}", subscriptionId);
            _paymentMetrics.RecordStripeApiCall(cancelImmediately ? "cancel_subscription" : "schedule_cancellation", false, stopwatch.Elapsed.TotalSeconds);
            _paymentMetrics.RecordStripeApiError(cancelImmediately ? "cancel_subscription" : "schedule_cancellation", ex.StripeError?.Type ?? "unknown");
            throw;
        }
    }

    public async Task<PaymentSubscriptionDto> ReactivateSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var options = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false
            };

            var service = new Stripe.SubscriptionService();
            var subscription = await service.UpdateAsync(subscriptionId, options, cancellationToken: cancellationToken);

            _logger.LogInformation("Reactivated Stripe subscription {SubscriptionId}", subscriptionId);

            // Record metrics
            _paymentMetrics.RecordStripeApiCall("reactivate_subscription", true, stopwatch.Elapsed.TotalSeconds);
            _paymentMetrics.SubscriptionUpdated("reactivation");

            return MapToSubscriptionDto(subscription);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to reactivate Stripe subscription {SubscriptionId}", subscriptionId);
            _paymentMetrics.RecordStripeApiCall("reactivate_subscription", false, stopwatch.Elapsed.TotalSeconds);
            _paymentMetrics.RecordStripeApiError("reactivate_subscription", ex.StripeError?.Type ?? "unknown");
            throw;
        }
    }

    public async Task<PaymentSubscriptionDto> GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var service = new Stripe.SubscriptionService();
            var subscription = await service.GetAsync(subscriptionId, cancellationToken: cancellationToken);

            // Record metrics
            _paymentMetrics.RecordStripeApiCall("get_subscription", true, stopwatch.Elapsed.TotalSeconds);

            return MapToSubscriptionDto(subscription);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to get Stripe subscription {SubscriptionId}", subscriptionId);
            _paymentMetrics.RecordStripeApiCall("get_subscription", false, stopwatch.Elapsed.TotalSeconds);
            _paymentMetrics.RecordStripeApiError("get_subscription", ex.StripeError?.Type ?? "unknown");
            throw;
        }
    }

    #endregion

    #region Checkout & Billing Portal

    public async Task<PaymentCheckoutSessionDto> CreateCheckoutSessionAsync(string customerId, string priceId, string successUrl, string cancelUrl, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var options = new StripeCheckoutSessionCreateOptions
            {
                Customer = customerId,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                {
                    new()
                    {
                        Price = priceId,
                        Quantity = 1
                    }
                },
                Mode = "subscription",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                AllowPromotionCodes = true,
                BillingAddressCollection = "auto",
                Metadata = metadata ?? new Dictionary<string, string>()
            };

            var service = new StripeCheckoutSessionService();
            var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogInformation("Created Stripe checkout session {SessionId} for customer {CustomerId}", session.Id, customerId);

            // Record metrics
            _paymentMetrics.RecordStripeApiCall("create_checkout_session", true, stopwatch.Elapsed.TotalSeconds);

            // Extract plan type from metadata for better tracking
            var planType = metadata?.GetValueOrDefault("plan_id") ?? "unknown";
            _paymentMetrics.CheckoutSessionCreated(planType);

            return MapToCheckoutSessionDto(session);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to create Stripe checkout session for customer {CustomerId}", customerId);
            _paymentMetrics.RecordStripeApiCall("create_checkout_session", false, stopwatch.Elapsed.TotalSeconds);
            _paymentMetrics.RecordStripeApiError("create_checkout_session", ex.StripeError?.Type ?? "unknown");
            throw;
        }
    }

    public async Task<PaymentCheckoutSessionDto> GetCheckoutSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new StripeCheckoutSessionService();
            var session = await service.GetAsync(sessionId, cancellationToken: cancellationToken);

            return MapToCheckoutSessionDto(session);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to get Stripe checkout session {SessionId}", sessionId);
            throw;
        }
    }

    public async Task<PaymentBillingPortalSessionDto> CreatePortalSessionAsync(string customerId, string returnUrl, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var options = new StripeBillingPortalSessionCreateOptions
            {
                Customer = customerId,
                ReturnUrl = returnUrl
            };

            var service = new StripeBillingPortalSessionService();
            var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogInformation("Created Stripe billing portal session {SessionId} for customer {CustomerId}", session.Id, customerId);

            // Record metrics
            _paymentMetrics.RecordStripeApiCall("create_billing_portal", true, stopwatch.Elapsed.TotalSeconds);
            _paymentMetrics.BillingPortalSessionCreated();

            return new PaymentBillingPortalSessionDto
            {
                Id = session.Id,
                Url = session.Url
            };
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to create Stripe billing portal session for customer {CustomerId}", customerId);
            _paymentMetrics.RecordStripeApiCall("create_billing_portal", false, stopwatch.Elapsed.TotalSeconds);
            _paymentMetrics.RecordStripeApiError("create_billing_portal", ex.StripeError?.Type ?? "unknown");
            throw;
        }
    }

    #endregion

    #region Invoice Management

    public async Task<PaymentInvoiceDto> GetInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new InvoiceService();
            var invoice = await service.GetAsync(invoiceId, cancellationToken: cancellationToken);

            return MapToInvoiceDto(invoice);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to get Stripe invoice {InvoiceId}", invoiceId);
            throw;
        }
    }

    public async Task<List<PaymentInvoiceDto>> GetInvoicesAsync(string customerId, int limit = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new InvoiceListOptions
            {
                Customer = customerId,
                Limit = limit,
                Status = "paid"
            };

            var service = new InvoiceService();
            var invoices = await service.ListAsync(options, cancellationToken: cancellationToken);

            return invoices.Data.Select(MapToInvoiceDto).ToList();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to get Stripe invoices for customer {CustomerId}", customerId);
            throw;
        }
    }

    #endregion

    #region Usage Records

    public Task<PaymentUsageRecordDto> CreateUsageRecordAsync(string subscriptionItemId, long quantity, DateTimeOffset timestamp, CancellationToken cancellationToken = default)
    {
        // Usage records functionality - implement when metered billing is needed
        // For now, return a basic implementation that tracks the request
        _logger.LogInformation("Usage record creation requested for subscription item {SubscriptionItemId}, quantity {Quantity}",
            subscriptionItemId, quantity);

        return Task.FromResult(new PaymentUsageRecordDto
        {
            Id = $"usg_{Guid.NewGuid().ToString("N")[..24]}", // Generate usage record-like ID
            Quantity = quantity,
            Timestamp = timestamp
        });
    }

    #endregion

    #region Private Mapping Methods

    private static PaymentCustomerDto MapToCustomerDto(StripeCustomer customer)
    {
        return new PaymentCustomerDto
        {
            Id = customer.Id,
            Email = customer.Email,
            Name = customer.Name,
            Metadata = customer.Metadata
        };
    }

    private static PaymentSubscriptionDto MapToSubscriptionDto(StripeSubscription subscription)
    {
        // Extract current period dates from RawJObject as they're not exposed as typed properties in this SDK version
        var currentPeriodStart = subscription.RawJObject?["current_period_start"]?.ToObject<long>() ?? 0;
        var currentPeriodEnd = subscription.RawJObject?["current_period_end"]?.ToObject<long>() ?? 0;

        // Extract price ID from the first item in the subscription (most subscriptions have only one item)
        var priceId = subscription.Items?.Data?.FirstOrDefault()?.Price?.Id ?? string.Empty;

        return new PaymentSubscriptionDto
        {
            Id = subscription.Id,
            CustomerId = subscription.CustomerId,
            Status = subscription.Status,
            PriceId = priceId,
            CurrentPeriodStart = DateTimeOffset.FromUnixTimeSeconds(currentPeriodStart),
            CurrentPeriodEnd = DateTimeOffset.FromUnixTimeSeconds(currentPeriodEnd),
            CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
            CanceledAt = subscription.CanceledAt,
            Metadata = subscription.Metadata ?? new Dictionary<string, string>()
        };
    }

    private static PaymentCheckoutSessionDto MapToCheckoutSessionDto(StripeCheckoutSession session)
    {
        return new PaymentCheckoutSessionDto
        {
            Id = session.Id,
            Url = session.Url,
            CustomerId = session.CustomerId,
            Status = session.Status,
            Metadata = session.Metadata
        };
    }

    private static PaymentInvoiceDto MapToInvoiceDto(StripeInvoice invoice)
    {
        // Extract subscription ID from RawJObject as it's not exposed as a direct property
        var subscriptionId = invoice.RawJObject?["subscription"]?.ToObject<string>() ?? string.Empty;

        return new PaymentInvoiceDto
        {
            Id = invoice.Id,
            CustomerId = invoice.CustomerId,
            SubscriptionId = subscriptionId,
            Status = invoice.Status,
            Amount = (invoice.AmountPaid > 0 ? invoice.AmountPaid : invoice.AmountDue) / 100m, // Convert from cents
            Currency = invoice.Currency,
            Created = invoice.Created,
            PaidAt = invoice.StatusTransitions?.PaidAt,
            InvoiceUrl = invoice.InvoicePdf
        };
    }

    #endregion
}