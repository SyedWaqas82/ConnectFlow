namespace ConnectFlow.Domain.Entities;

public class Subscription : BaseAuditableEntity
{
    // Stripe and Billing
    public string StripeCustomerId { get; set; } = default!;
    public string StripeSubscriptionId { get; set; } = default!;
    public decimal Amount { get; set; } // Actual amount charged for this subscription
    public string Currency { get; set; } = "USD";
    public BillingCycle BillingCycle { get; set; } // Monthly, Yearly, etc.

    // Plan Reference
    public SubscriptionPlan Plan { get; set; }

    // Status & Lifecycle
    public SubscriptionStatus Status { get; set; }
    public DateTimeOffset? CurrentPeriodStartsAt { get; set; }
    public DateTimeOffset? CurrentPeriodEndsAt { get; set; }
    public DateTimeOffset? CanceledAt { get; set; }
    public DateTimeOffset? TrialEndsAt { get; set; } // If in trial period
    public bool CancelAtPeriodEnd { get; set; }
    public string? CancellationReason { get; set; }

    // Usage Limits (copied from plan at time of subscription, can be overridden)
    public int UserLimit { get; set; }
    public int LeadLimit { get; set; }
    public int ContactLimit { get; set; }
    public int CompanyLimit { get; set; }
    public int CustomFieldLimit { get; set; }
    public int MonthlyAITokenLimit { get; set; }
    public int AdditionalAITokens { get; set; } // Purchased extra tokens
    public int StorageLimitGB { get; set; } // Storage limit in GB

    // Tenant
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}