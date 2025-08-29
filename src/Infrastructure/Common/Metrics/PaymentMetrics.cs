using System.Diagnostics.Metrics;

namespace ConnectFlow.Infrastructure.Common.Metrics;

/// <summary>
/// Provides metrics tracking for payment and Stripe service operations
/// </summary>
public class PaymentMetrics
{
    private readonly Counter<long> _stripeApiCalls;
    private readonly Counter<long> _stripeApiErrors;
    private readonly Histogram<double> _stripeApiDuration;
    private readonly Counter<long> _webhooksReceived;
    private readonly Counter<long> _webhooksProcessed;
    private readonly Counter<long> _webhooksFailed;
    private readonly Histogram<double> _webhookProcessingDuration;
    private readonly Counter<long> _customersCreated;
    private readonly Counter<long> _subscriptionsCreated;
    private readonly Counter<long> _subscriptionsUpdated;
    private readonly Counter<long> _subscriptionsCanceled;
    private readonly Counter<long> _paymentsSuccessful;
    private readonly Counter<long> _paymentsFailed;
    private readonly Counter<long> _refundsProcessed;
    private readonly Histogram<double> _paymentAmounts;
    private readonly Counter<long> _checkoutSessionsCreated;
    private readonly Counter<long> _billingPortalSessionsCreated;
    private readonly Counter<long> _planChanges;

    public PaymentMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("ConnectFlow.Payment");

        // Stripe API metrics
        _stripeApiCalls = meter.CreateCounter<long>("stripe_api_calls_total", description: "Total number of Stripe API calls made");
        _stripeApiErrors = meter.CreateCounter<long>("stripe_api_errors_total", description: "Total number of Stripe API errors");
        _stripeApiDuration = meter.CreateHistogram<double>("stripe_api_duration_seconds", unit: "s", description: "Duration of Stripe API calls");

        // Webhook metrics
        _webhooksReceived = meter.CreateCounter<long>("stripe_webhooks_received_total", description: "Total number of Stripe webhooks received");
        _webhooksProcessed = meter.CreateCounter<long>("stripe_webhooks_processed_total", description: "Total number of Stripe webhooks processed successfully");
        _webhooksFailed = meter.CreateCounter<long>("stripe_webhooks_failed_total", description: "Total number of Stripe webhooks that failed processing");
        _webhookProcessingDuration = meter.CreateHistogram<double>("stripe_webhook_processing_duration_seconds", unit: "s", description: "Duration of Stripe webhook processing");

        // Customer metrics
        _customersCreated = meter.CreateCounter<long>("stripe_customers_created_total", description: "Total number of Stripe customers created");

        // Subscription metrics
        _subscriptionsCreated = meter.CreateCounter<long>("stripe_subscriptions_created_total", description: "Total number of Stripe subscriptions created");
        _subscriptionsUpdated = meter.CreateCounter<long>("stripe_subscriptions_updated_total", description: "Total number of Stripe subscriptions updated");
        _subscriptionsCanceled = meter.CreateCounter<long>("stripe_subscriptions_canceled_total", description: "Total number of Stripe subscriptions canceled");
        _planChanges = meter.CreateCounter<long>("stripe_plan_changes_total", description: "Total number of subscription plan changes");

        // Payment metrics
        _paymentsSuccessful = meter.CreateCounter<long>("stripe_payments_successful_total", description: "Total number of successful payments");
        _paymentsFailed = meter.CreateCounter<long>("stripe_payments_failed_total", description: "Total number of failed payments");
        _refundsProcessed = meter.CreateCounter<long>("stripe_refunds_processed_total", description: "Total number of refunds processed");
        _paymentAmounts = meter.CreateHistogram<double>("stripe_payment_amounts", unit: "USD", description: "Payment amounts processed");

        // Session metrics
        _checkoutSessionsCreated = meter.CreateCounter<long>("stripe_checkout_sessions_created_total", description: "Total number of Stripe checkout sessions created");
        _billingPortalSessionsCreated = meter.CreateCounter<long>("stripe_billing_portal_sessions_created_total", description: "Total number of Stripe billing portal sessions created");
    }

    #region Stripe API Metrics

    /// <summary>
    /// Record a Stripe API call
    /// </summary>
    /// <param name="operation">The API operation (e.g., "create_customer", "update_subscription")</param>
    /// <param name="success">Whether the call was successful</param>
    /// <param name="durationSeconds">Duration of the API call in seconds</param>
    public void RecordStripeApiCall(string operation, bool success, double durationSeconds)
    {
        var tags = new KeyValuePair<string, object?>("operation", operation);

        _stripeApiCalls.Add(1, tags);
        _stripeApiDuration.Record(durationSeconds, tags);

        if (!success)
        {
            _stripeApiErrors.Add(1, tags);
        }
    }

    /// <summary>
    /// Record a Stripe API error
    /// </summary>
    /// <param name="operation">The API operation</param>
    /// <param name="errorType">The type of error (e.g., "rate_limit", "invalid_request")</param>
    public void RecordStripeApiError(string operation, string errorType)
    {
        _stripeApiErrors.Add(1,
            new KeyValuePair<string, object?>("operation", operation),
            new KeyValuePair<string, object?>("error_type", errorType));
    }

    #endregion

    #region Webhook Metrics

    /// <summary>
    /// Record a webhook being received
    /// </summary>
    /// <param name="eventType">The Stripe event type</param>
    public void WebhookReceived(string eventType)
    {
        _webhooksReceived.Add(1, new KeyValuePair<string, object?>("event_type", eventType));
    }

    /// <summary>
    /// Record a webhook being processed successfully
    /// </summary>
    /// <param name="eventType">The Stripe event type</param>
    /// <param name="durationSeconds">Processing duration in seconds</param>
    public void WebhookProcessed(string eventType, double durationSeconds)
    {
        var tags = new KeyValuePair<string, object?>("event_type", eventType);

        _webhooksProcessed.Add(1, tags);
        _webhookProcessingDuration.Record(durationSeconds, tags);
    }

    /// <summary>
    /// Record a webhook processing failure
    /// </summary>
    /// <param name="eventType">The Stripe event type</param>
    /// <param name="errorType">The type of error</param>
    public void WebhookFailed(string eventType, string errorType)
    {
        _webhooksFailed.Add(1,
            new KeyValuePair<string, object?>("event_type", eventType),
            new KeyValuePair<string, object?>("error_type", errorType));
    }

    #endregion

    #region Customer Metrics

    /// <summary>
    /// Record a customer being created
    /// </summary>
    /// <param name="tenantId">The tenant ID for segmentation</param>
    public void CustomerCreated(int? tenantId = null)
    {
        var tags = tenantId.HasValue
            ? new KeyValuePair<string, object?>("tenant_id", tenantId.Value.ToString())
            : new KeyValuePair<string, object?>("tenant_id", "unknown");

        _customersCreated.Add(1, tags);
    }

    #endregion

    #region Subscription Metrics

    /// <summary>
    /// Record a subscription being created
    /// </summary>
    /// <param name="planType">The subscription plan type</param>
    /// <param name="amount">The subscription amount</param>
    public void SubscriptionCreated(string planType, decimal? amount = null)
    {
        _subscriptionsCreated.Add(1, new KeyValuePair<string, object?>("plan_type", planType));

        if (amount.HasValue)
        {
            _paymentAmounts.Record((double)amount.Value, new KeyValuePair<string, object?>("type", "subscription"));
        }
    }

    /// <summary>
    /// Record a subscription being updated
    /// </summary>
    /// <param name="changeType">Type of change (e.g., "plan_upgrade", "plan_downgrade", "quantity_change")</param>
    /// <param name="fromPlan">Previous plan type</param>
    /// <param name="toPlan">New plan type</param>
    public void SubscriptionUpdated(string changeType, string? fromPlan = null, string? toPlan = null)
    {
        _subscriptionsUpdated.Add(1, new KeyValuePair<string, object?>("change_type", changeType));

        if (!string.IsNullOrEmpty(fromPlan) && !string.IsNullOrEmpty(toPlan))
        {
            _planChanges.Add(1,
                new KeyValuePair<string, object?>("from_plan", fromPlan),
                new KeyValuePair<string, object?>("to_plan", toPlan));
        }
    }

    /// <summary>
    /// Record a subscription being canceled
    /// </summary>
    /// <param name="cancelationType">Type of cancellation (e.g., "immediate", "at_period_end")</param>
    /// <param name="planType">The subscription plan type</param>
    public void SubscriptionCanceled(string cancelationType, string planType)
    {
        _subscriptionsCanceled.Add(1,
            new KeyValuePair<string, object?>("cancellation_type", cancelationType),
            new KeyValuePair<string, object?>("plan_type", planType));
    }

    #endregion

    #region Payment Metrics

    /// <summary>
    /// Record a successful payment
    /// </summary>
    /// <param name="amount">Payment amount</param>
    /// <param name="currency">Payment currency</param>
    /// <param name="paymentType">Type of payment (e.g., "subscription", "one_time")</param>
    public void PaymentSuccessful(decimal amount, string currency = "USD", string paymentType = "subscription")
    {
        _paymentsSuccessful.Add(1,
            new KeyValuePair<string, object?>("currency", currency),
            new KeyValuePair<string, object?>("payment_type", paymentType));

        _paymentAmounts.Record((double)amount,
            new KeyValuePair<string, object?>("currency", currency),
            new KeyValuePair<string, object?>("type", paymentType));
    }

    /// <summary>
    /// Record a failed payment
    /// </summary>
    /// <param name="amount">Payment amount</param>
    /// <param name="failureReason">Reason for failure</param>
    /// <param name="currency">Payment currency</param>
    /// <param name="paymentType">Type of payment</param>
    public void PaymentFailed(decimal amount, string failureReason, string currency = "USD", string paymentType = "subscription")
    {
        _paymentsFailed.Add(1,
            new KeyValuePair<string, object?>("failure_reason", failureReason),
            new KeyValuePair<string, object?>("currency", currency),
            new KeyValuePair<string, object?>("payment_type", paymentType));
    }

    /// <summary>
    /// Record a refund being processed
    /// </summary>
    /// <param name="amount">Refund amount</param>
    /// <param name="refundType">Type of refund (e.g., "full", "partial")</param>
    /// <param name="currency">Refund currency</param>
    public void RefundProcessed(decimal amount, string refundType, string currency = "USD")
    {
        _refundsProcessed.Add(1,
            new KeyValuePair<string, object?>("refund_type", refundType),
            new KeyValuePair<string, object?>("currency", currency));
    }

    #endregion

    #region Session Metrics

    /// <summary>
    /// Record a checkout session being created
    /// </summary>
    /// <param name="planType">The plan type for the checkout</param>
    public void CheckoutSessionCreated(string planType)
    {
        _checkoutSessionsCreated.Add(1, new KeyValuePair<string, object?>("plan_type", planType));
    }

    /// <summary>
    /// Record a billing portal session being created
    /// </summary>
    public void BillingPortalSessionCreated()
    {
        _billingPortalSessionsCreated.Add(1);
    }

    #endregion
}