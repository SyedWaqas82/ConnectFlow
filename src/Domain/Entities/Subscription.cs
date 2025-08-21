namespace ConnectFlow.Domain.Entities;

public class Subscription : BaseAuditableEntity
{
    public string StripeSubscriptionId { get; set; } = string.Empty;
    public SubscriptionStatus Status { get; set; }
    public DateTimeOffset CurrentPeriodStart { get; set; }
    public DateTimeOffset CurrentPeriodEnd { get; set; }
    public DateTimeOffset? CanceledAt { get; set; }
    public DateTimeOffset? CancelAtPeriodEnd { get; set; }

    // Plan
    public int PlanId { get; set; }
    public Plan Plan { get; set; } = null!;

    // Tenant
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}