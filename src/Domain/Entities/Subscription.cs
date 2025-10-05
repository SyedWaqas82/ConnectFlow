namespace ConnectFlow.Domain.Entities;

public class Subscription : BaseAuditableEntity
{
    public string PaymentProviderSubscriptionId { get; set; } = string.Empty;
    public SubscriptionStatus Status { get; set; }
    public DateTimeOffset CurrentPeriodStart { get; set; }
    public DateTimeOffset CurrentPeriodEnd { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
    public DateTimeOffset? CanceledAt { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public DateTimeOffset? CancellationRequestedAt { get; set; } // When cancellation was originally requested

    // Grace Period and payment failure tracking
    public bool IsInGracePeriod { get; set; }
    public DateTimeOffset? GracePeriodEndsAt { get; set; }
    public DateTimeOffset? FirstPaymentFailureAt { get; set; }
    public DateTimeOffset? LastPaymentFailedAt { get; set; }
    public int PaymentRetryCount { get; set; } = 0;
    public bool HasReachedMaxRetries { get; set; } = false;

    // Plan
    public int PlanId { get; set; }
    public Plan Plan { get; set; } = null!;

    // Tenant
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}