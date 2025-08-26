namespace ConnectFlow.Domain.Entities;

public class Subscription : BaseAuditableEntity
{
    public string PaymentProviderSubscriptionId { get; set; } = string.Empty;
    public SubscriptionStatus Status { get; set; }
    public DateTimeOffset CurrentPeriodStart { get; set; }
    public DateTimeOffset CurrentPeriodEnd { get; set; }
    public DateTimeOffset? CanceledAt { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public DateTimeOffset? CancellationRequestedAt { get; set; } // When cancellation was originally requested

    // Grace Period for failed payments
    public DateTimeOffset? GracePeriodEndsAt { get; set; }
    public bool IsInGracePeriod { get; set; }
    public DateTimeOffset? LastPaymentFailedAt { get; set; }

    // Payment retry tracking
    public int PaymentRetryCount { get; set; } = 0;
    public DateTimeOffset? FirstPaymentFailureAt { get; set; }
    public DateTimeOffset? NextRetryAt { get; set; }
    public bool HasReachedMaxRetries { get; set; } = false;

    // Plan
    public int PlanId { get; set; }
    public Plan Plan { get; set; } = null!;

    // Tenant
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public IList<Invoice> Invoices { get; private set; } = new List<Invoice>();
}