# ConnectFlow Payment & Subscription System

This document provides a comprehensive guide to the payment, subscription, and Stripe integration system in ConnectFlow.

## 1. System Overview

ConnectFlow implements a robust subscription management system with Stripe integration, supporting multi-tenant SaaS billing, payment processing, and subscription lifecycle management.

### 1.1 Architecture Components

The payment system consists of these key components:

```ascii
┌──────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   Frontend   │────►│   Application   │────►│ Infrastructure  │
│   (Checkout) │     │     Layer       │     │     Layer       │
└──────────────┘     └─────────────────┘     └─────────────────┘
       ▲                       ▲                       ▲
       │                       │                       │
┌──────┴──────┐      ┌─────────┴────────┐    ┌─────────┴─────────┐
│   Stripe    │      │   Domain Events  │    │   Stripe Service  │
│  Checkout   │      │   & Handlers     │    │   & Webhooks     │
│   Portal    │      └──────────────────┘    └───────────────────┘
└─────────────┘              ▲                         ▲
       ▲                     │                         │
       │            ┌────────┴─────────┐    ┌─────────┴─────────┐
       └────────────│   Event Bus      │    │    Database      │
                    │  (MediatR)       │    │   (PostgreSQL)   │
                    └──────────────────┘    └───────────────────┘
```

### 1.2 Core Features

- **Multi-tenant subscription management**
- **Stripe payment processing**
- **Webhook event handling**
- **Payment retry logic**
- **Grace period management**
- **Plan upgrades/downgrades**
- **Prorated billing**
- **Comprehensive metrics collection**
- **Domain-driven event system**

## 2. Domain Model

### 2.1 Core Entities

#### Subscription Entity
```csharp
public class Subscription : BaseAuditableEntity
{
    public string PaymentProviderSubscriptionId { get; set; }  // Stripe subscription ID
    public SubscriptionStatus Status { get; set; }
    public DateTimeOffset CurrentPeriodStart { get; set; }
    public DateTimeOffset CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public DateTimeOffset? CanceledAt { get; set; }
    public DateTimeOffset? CancellationRequestedAt { get; set; }
    
    // Payment retry tracking
    public int PaymentRetryCount { get; set; }
    public DateTimeOffset? FirstPaymentFailureAt { get; set; }
    public DateTimeOffset? LastPaymentFailedAt { get; set; }
    public DateTimeOffset? NextRetryAt { get; set; }
    public bool HasReachedMaxRetries { get; set; }
    
    // Grace period management
    public bool IsInGracePeriod { get; set; }
    public DateTimeOffset? GracePeriodEndsAt { get; set; }
    
    // Relationships
    public int TenantId { get; set; }
    public int PlanId { get; set; }
    public Plan Plan { get; set; }
    public Tenant Tenant { get; set; }
}
```

#### Plan Entity
```csharp
public class Plan : BaseAuditableEntity
{
    public string Name { get; set; }
    public PlanType Type { get; set; }
    public decimal Price { get; set; }
    public BillingInterval BillingInterval { get; set; }
    public string PaymentProviderPriceId { get; set; }  // Stripe price ID
    public bool IsActive { get; set; }
    public Dictionary<string, object> Features { get; set; }
    public Dictionary<string, int> Limits { get; set; }
}
```

#### Tenant Entity
```csharp
public class Tenant : BaseAuditableEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PaymentProviderCustomerId { get; set; }  // Stripe customer ID
    public TenantStatus Status { get; set; }
    public List<Subscription> Subscriptions { get; set; }
}
```

### 2.2 Enumerations

```csharp
public enum SubscriptionStatus
{
    Active,
    Canceled,
    PastDue,
    Unpaid,
    Trialing,
    Incomplete,
    IncompleteExpired
}

public enum PlanType
{
    Free,
    Basic,
    Professional,
    Enterprise
}

public enum PaymentAction
{
    Success,
    Failed,
    Retry,
    Refunded,
    PartialRefund
}
```

## 3. Application Layer

### 3.1 Commands

#### CreateSubscriptionCommand
Creates a new subscription and Stripe checkout session.

```csharp
[AuthorizeTenant(false, false, Roles.TenantAdmin)]
public record CreateSubscriptionCommand : IRequest<CreateSubscriptionResult>
{
    public int PlanId { get; init; }
    public string SuccessUrl { get; init; } = string.Empty;
    public string CancelUrl { get; init; } = string.Empty;
}
```

**Flow:**
1. Validate plan exists and is active
2. Check tenant has Stripe customer ID
3. Create Stripe customer if needed
4. Create Stripe checkout session
5. Return checkout URL for payment

#### UpdateSubscriptionCommand
Updates existing subscription (plan changes, cancellation).

```csharp
[AuthorizeTenant(true, false, Roles.TenantAdmin)]
public record UpdateSubscriptionCommand : IRequest<UpdateSubscriptionResult>
{
    public int PlanId { get; init; }
    public bool ProrateBilling { get; init; } = true;
}
```

#### CancelSubscriptionCommand
Cancels subscription at period end or immediately.

```csharp
[AuthorizeTenant(true, false, Roles.TenantAdmin)]
public record CancelSubscriptionCommand : IRequest<CancelSubscriptionResult>
{
    public bool CancelImmediately { get; init; } = false;
    public string Reason { get; init; } = string.Empty;
}
```

### 3.2 Queries

#### GetSubscriptionQuery
Retrieves current subscription with usage statistics.

#### GetCheckoutSessionQuery
Checks status of Stripe checkout session.

```csharp
[AuthorizeTenant(true, false, Roles.TenantAdmin)]
public record GetCheckoutSessionQuery : IRequest<CheckoutSessionStatusDto>
{
    public string SessionId { get; init; } = string.Empty;
}
```

### 3.3 Event Handlers

#### PaymentStatusEventHandler
Handles payment success/failure events and manages subscription state.

```csharp
public class PaymentStatusEventHandler : INotificationHandler<PaymentStatusEvent>
{
    // Handles:
    // - Payment success: Reset retry counters, reactivate subscription
    // - Payment failure: Update retry tracking, start grace period
    // - Grace period logic: Calculate intelligent grace periods
    // - Email notifications: Send payment status emails
}
```

#### SubscriptionStatusEventHandler
Handles subscription lifecycle events.

```csharp
public class SubscriptionStatusEventHandler : INotificationHandler<SubscriptionStatusEvent>
{
    // Handles:
    // - Subscription creation/cancellation
    // - Plan changes and upgrades/downgrades
    // - Suspension and reactivation
    // - Grace period start/end
    // - Email notifications
}
```

## 4. Infrastructure Layer

### 4.1 Stripe Service

The `StripeService` implements `IPaymentService` and handles all Stripe API interactions.

#### Key Methods

```csharp
public class StripeService : IPaymentService
{
    // Customer Management
    Task<PaymentCustomerDto> CreateCustomerAsync(string email, string name, ...);
    Task<PaymentCustomerDto> UpdateCustomerAsync(string customerId, ...);
    
    // Subscription Management
    Task<PaymentSubscriptionDto> CreateSubscriptionAsync(string customerId, string priceId, ...);
    Task<PaymentSubscriptionDto> UpdateSubscriptionAsync(string subscriptionId, ...);
    Task<PaymentSubscriptionDto> CancelSubscriptionAsync(string subscriptionId, bool cancelImmediately);
    
    // Checkout Sessions
    Task<PaymentCheckoutSessionDto> CreateCheckoutSessionAsync(string customerId, string priceId, ...);
    Task<PaymentCheckoutSessionDto> GetCheckoutSessionAsync(string sessionId, ...);
    
    // Webhooks
    Task<PaymentEventDto> ProcessWebhookAsync(string body, string signature, ...);
}
```

### 4.2 Webhook Event Handler

The `StripeWebhookEventHandlerService` processes individual webhook events.

#### Supported Webhook Events

```csharp
public class StripeWebhookEventHandlerService : IPaymentWebhookEventHandlerService
{
    // Subscription Events
    Task<bool> HandleSubscriptionCreatedAsync(PaymentEventDto paymentEvent, ...);
    Task<bool> HandleSubscriptionUpdatedAsync(PaymentEventDto paymentEvent, ...);
    Task<bool> HandleSubscriptionDeletedAsync(PaymentEventDto paymentEvent, ...);
    
    // Invoice Events
    Task<bool> HandleInvoicePaymentSucceededAsync(PaymentEventDto paymentEvent, ...);
    Task<bool> HandleInvoicePaymentFailedAsync(PaymentEventDto paymentEvent, ...);
    Task<bool> HandleInvoiceFinalizedAsync(PaymentEventDto paymentEvent, ...);
    
    // Checkout Events
    Task<bool> HandleCheckoutSessionCompletedAsync(PaymentEventDto paymentEvent, ...);
}
```

## 5. Event System

### 5.1 Domain Events

#### PaymentStatusEvent
Triggered when payment status changes.

```csharp
public class PaymentStatusEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public PaymentAction Action { get; }
    public string Reason { get; }
    public decimal? Amount { get; }
    public bool SendEmailNotification { get; }
    public int FailureCount { get; }
    public DateTimeOffset Timestamp { get; }
}
```

#### SubscriptionStatusEvent
Triggered for subscription lifecycle changes.

```csharp
public class SubscriptionStatusEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public SubscriptionAction Action { get; }
    public string Reason { get; }
    public bool SendEmailNotification { get; }
    public bool SuspendLimitsImmediately { get; }
    public int? PreviousPlanId { get; }
    public int? NewPlanId { get; }
}
```

### 5.2 Event Flow

```ascii
Stripe Webhook → StripeWebhookEventHandlerService → Domain Events → Event Handlers
     ▲                        ▲                          ▲              ▲
     │                        │                          │              │
     │                   Process Event                   │         Update State
     │                   Create Domain Event             │         Send Emails
     │                   Update Database                 │         Trigger Actions
     │                                                   │
     └─────────────────── Webhook Response ──────────────┘
```

## 6. Webhook Processing

### 6.1 Webhook Endpoint

**Endpoint:** `POST /subscriptions/ProcessWebhook`
- **Authentication:** None (verified via Stripe signature)
- **Content-Type:** `application/json`
- **Headers Required:** `Stripe-Signature`

### 6.2 Webhook Security

```csharp
public async Task<PaymentEventDto> ProcessWebhookAsync(string body, string signature, ...)
{
    try
    {
        // Verify webhook signature using Stripe webhook secret
        var stripeEvent = EventUtility.ConstructEvent(body, signature, _stripeSettings.WebhookSecret);
        
        // Process event and return DTO
        return MapToPaymentEventDto(stripeEvent);
    }
    catch (StripeException ex) when (ex.StripeError?.Type == "invalid_request_error")
    {
        // Invalid signature - return 400 (don't retry)
        throw new ArgumentException("Invalid webhook signature");
    }
}
```

### 6.3 Webhook Event Routing

```csharp
public async Task<ProcessWebhookResult> Handle(ProcessWebhookCommand request, ...)
{
    var success = request.EventType switch
    {
        "customer.subscription.created" => await _eventHandler.HandleSubscriptionCreatedAsync(...),
        "customer.subscription.updated" => await _eventHandler.HandleSubscriptionUpdatedAsync(...),
        "customer.subscription.deleted" => await _eventHandler.HandleSubscriptionDeletedAsync(...),
        "invoice.payment_succeeded" => await _eventHandler.HandleInvoicePaymentSucceededAsync(...),
        "invoice.payment_failed" => await _eventHandler.HandleInvoicePaymentFailedAsync(...),
        "invoice.finalized" => await _eventHandler.HandleInvoiceFinalizedAsync(...),
        "checkout.session.completed" => await _eventHandler.HandleCheckoutSessionCompletedAsync(...),
        _ => await _eventHandler.HandleUnknownEventAsync(...)
    };
}
```

## 7. Payment Retry Logic

### 7.1 Stripe Retry Schedule

Stripe automatically retries failed payments according to this schedule:
- **1st retry:** 3-5 days after initial failure
- **2nd retry:** 5-7 days after first retry  
- **3rd retry:** 7-9 days after second retry
- **4th retry:** 10-11 days after third retry

### 7.2 Grace Period Logic

#### Standard Grace Period
```csharp
var gracePeriodEnd = DateTimeOffset.UtcNow.AddDays(_subscriptionSettings.GracePeriodDays);
```

#### Intelligent Grace Period
```csharp
private DateTimeOffset CalculateIntelligentGracePeriodEnd(int attemptCount, DateTimeOffset firstFailureAt)
{
    var baseGracePeriodEnd = DateTimeOffset.UtcNow.AddDays(_subscriptionSettings.GracePeriodDays);
    var stripeRetryEndTime = firstFailureAt.AddDays(_subscriptionSettings.StripeRetryPeriodDays);
    var intelligentEndTime = stripeRetryEndTime.AddHours(_subscriptionSettings.RetryAttemptGracePeriodHours);
    
    return baseGracePeriodEnd > intelligentEndTime ? baseGracePeriodEnd : intelligentEndTime;
}
```

### 7.3 Retry Flow

```ascii
Payment Fails → Update Retry Count → Calculate Next Retry → Start Grace Period?
     ▲                 ▲                      ▲                    ▲
     │                 │                      │                    │
     │            Track in DB            Use Stripe          Check Settings
     │            Send Email           Schedule Logic       Suspend if Needed
     │                                                            │
     └─────────────── Continue until Max Retries ────────────────┘
```

## 8. Configuration

### 8.1 Subscription Settings

```json
{
  "SubscriptionSettings": {
    "GracePeriodDays": 7,                    // Grace period after payment failure
    "MaxPaymentRetries": 4,                  // Maximum retry attempts (matches Stripe)
    "StripeRetryPeriodDays": 25,            // Stripe's full retry period
    "DefaultDowngradePlanName": "Free",      // Plan to downgrade to
    "AutoDowngradeAfterGracePeriod": true,   // Auto-downgrade after grace period
    "AutoDowngradeAfterMaxRetries": false,   // Don't auto-downgrade on max retries
    "EventIdempotencyCacheDuration": 24,     // Cache webhook events (hours)
    "UseIntelligentGracePeriod": true,       // Extend grace period beyond Stripe retries
    "RetryAttemptGracePeriodHours": 72       // Buffer hours after last retry
  }
}
```

### 8.2 Stripe Settings

```json
{
  "StripeSettings": {
    "SecretKey": "sk_test_...",              // Stripe secret key
    "WebhookSecret": "whsec_..."             // Webhook endpoint secret
  }
}
```

## 9. Metrics & Monitoring

### 9.1 Payment Metrics

The system collects comprehensive metrics via `PaymentMetrics`:

```csharp
public class PaymentMetrics
{
    // Payment tracking
    void PaymentSuccessful(decimal amount, string currency = "USD", string paymentType = "subscription");
    void PaymentFailed(decimal amount, string failureReason, string currency = "USD", string paymentType = "subscription");
    
    // Stripe API metrics
    void RecordStripeApiCall(string operation, bool success, double durationSeconds);
    void RecordStripeApiError(string operation, string errorType);
    
    // Webhook metrics
    void WebhookReceived(string eventType);
    void WebhookProcessed(string eventType, double processingTimeSeconds);
    void WebhookFailed(string eventType, string errorType);
    
    // Subscription metrics
    void SubscriptionCreated(string planType, decimal? amount = null);
    void SubscriptionCanceled(string cancelationType, string planType);
    void SubscriptionUpdated(string changeType, string fromPlan, string toPlan);
}
```

### 9.2 Grafana Dashboard

The payment dashboard provides visualization for:
- **Payment success/failure rates**
- **Revenue tracking**
- **Stripe API performance**
- **Webhook processing metrics**
- **Subscription lifecycle events**
- **Failure reason breakdown**

## 10. Email Notifications

### 10.1 Email Templates

The system supports various email notifications:

```csharp
public static class EmailTemplates
{
    public const string PaymentSuccess = "payment-success";
    public const string PaymentFailed = "payment-failed";
    public const string PaymentRetry = "payment-retry";
    public const string PaymentRefunded = "payment-refunded";
    public const string SubscriptionCreated = "subscription-created";
    public const string SubscriptionCanceled = "subscription-canceled";
    public const string SubscriptionUpgraded = "subscription-upgraded";
    public const string SubscriptionDowngraded = "subscription-downgraded";
    public const string GracePeriodStarted = "grace-period-started";
    public const string SubscriptionSuspended = "subscription-suspended";
}
```

### 10.2 Email Data

```csharp
var emailEvent = new EmailSendMessageEvent(tenantId, userId)
{
    To = subscription.Tenant.Email,
    Subject = GetEmailSubject(notification.Action),
    TemplateId = GetEmailTemplateId(notification.Action),
    TemplateData = new Dictionary<string, object>
    {
        { "tenantName", subscription.Tenant.Name },
        { "amount", notification.Amount?.ToString("C") },
        { "planName", subscription.Plan?.Name },
        { "nextRetryDate", subscription.NextRetryAt?.ToString("MMMM dd, yyyy") },
        { "failureCount", notification.FailureCount }
    }
};
```

## 11. API Endpoints


### 11.1 Subscription Management

```http
GET    /subscriptions/GetSubscription           # Get current subscription (auth required)
GET    /subscriptions/GetAvailablePlans         # Get available plans (anonymous allowed)
GET    /subscriptions/GetCheckoutSession/{sessionId}   # Check checkout session status (auth required)
POST   /subscriptions/Create                    # Create new subscription (auth required)
PUT    /subscriptions/Update                    # Update subscription (auth required)
POST   /subscriptions/Cancel                    # Cancel subscription (auth required)
POST   /subscriptions/Reactivate                # Reactivate subscription (auth required)
POST   /subscriptions/ProcessWebhook            # Stripe webhook endpoint (anonymous allowed)
```

### 11.2 Authentication


All endpoints except `GetAvailablePlans` and `ProcessWebhook` require:

- **JWT Bearer token**
- **Tenant context** (X-Tenant-Id header)
- **TenantAdmin role**


## 12. Error Handling

### 12.1 Webhook Error Responses

```csharp
// Success - Stripe won't retry
return TypedResults.Ok(result);  // 200

// Client error - Stripe won't retry
return TypedResults.BadRequest(error);  // 400

// Server error - Stripe will retry
return TypedResults.Problem(title: "Webhook processing failed", statusCode: 500);  // 500
```

### 12.2 Common Error Scenarios

1. **Invalid webhook signature** → 400 Bad Request
2. **Missing Stripe customer** → Create customer automatically
3. **Subscription not found** → Log warning, return success
4. **Plan not found** → Log error, return 500 for retry
5. **Database timeout** → Return 500 for retry

## 13. Security Considerations

### 13.1 Webhook Security

- **Signature verification** using Stripe webhook secret
- **Tenant isolation** - verify webhook data belongs to correct tenant
- **Idempotency** - cache events to prevent duplicate processing
- **Rate limiting** on webhook endpoint

### 13.2 API Security

- **JWT authentication** for all subscription endpoints
- **Tenant authorization** - users can only access their tenant's data
- **Role-based access** - TenantAdmin required for subscription changes
- **Input validation** using FluentValidation

## 14. Testing

### 14.1 Stripe Test Mode


Use Stripe test mode for development:

- **Test cards** for simulating payments
- **Webhook testing** using Stripe CLI
- **Test clock** for simulating time-based scenarios

#### Testing Webhook Events with Stripe CLI

You can use the Stripe CLI to test webhook event delivery to your local or remote endpoint. This is useful for simulating real Stripe events and verifying your webhook processing logic.

**Steps:**

1. Install the Stripe CLI:
     - With Homebrew (macOS):
         ```bash
         brew install stripe/stripe-cli/stripe
         ```
     - Or see the [official Stripe CLI docs](https://stripe.com/docs/stripe-cli) for other platforms.
2. Log in to Stripe CLI:
    ```bash
    stripe login
    ```
3. Forward webhook events to your local endpoint (replace the URL with your actual endpoint):
    ```bash
    stripe listen --forward-to localhost:5001/api/v1/subscriptions/ProcessWebhook
    ```
4. Trigger a test event (e.g., subscription created):
    ```bash
    stripe trigger customer.subscription.created
    ```
5. Check your application logs and the Stripe CLI output to verify the event was received and processed.

**Common test events:**

- `customer.subscription.created`
- `customer.subscription.updated`
- `customer.subscription.deleted`
- `invoice.payment_succeeded`
- `invoice.payment_failed`
- `checkout.session.completed`

See the [Stripe CLI trigger reference](https://stripe.com/docs/stripe-cli/events#trigger) for a full list of supported test events.


### 14.2 Test Scenarios

1. **Successful payment flow**
2. **Failed payment retry logic**
3. **Subscription cancellation**
4. **Plan upgrades/downgrades**
5. **Grace period expiration**
6. **Webhook failure handling**

## 15. Deployment Considerations

### 15.1 Environment Configuration

- **Stripe keys** - Use different keys per environment
- **Webhook endpoints** - Configure separate endpoints
- **Database migrations** - Ensure subscription tables exist
- **Email templates** - Deploy email templates

### 15.2 Monitoring Setup

- **Webhook endpoint health** - Monitor for webhook delivery failures
- **Payment metrics** - Set up alerts for high failure rates
- **Database performance** - Monitor subscription query performance
- **Stripe API limits** - Monitor API usage

## 16. Troubleshooting

### 16.1 Common Issues

1. **Webhook delivery failures**
   - Check webhook endpoint URL
   - Verify webhook secret configuration
   - Check server logs for errors

2. **Payment retry not working**
   - Verify Stripe retry settings
   - Check subscription retry tracking fields
   - Review PaymentStatusEventHandler logs

3. **Email notifications not sent**
   - Check email service configuration
   - Verify email templates exist
   - Review RabbitMQ message delivery

### 16.2 Debugging Tools

- **Stripe Dashboard** - View webhook delivery attempts
- **Application logs** - Structured logging with correlation IDs
- **Grafana dashboards** - Real-time metrics and alerts
- **Database queries** - Check subscription state directly

---

This documentation provides a complete overview of the ConnectFlow payment and subscription system. For specific implementation details, refer to the source code and inline documentation.
