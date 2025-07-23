namespace ConnectFlow.Domain.Entities;

public class Subscription : BaseAuditableEntity
{
    public string StripeCustomerId { get; set; } = default!;
    public string StripeSubscriptionId { get; set; } = default!;
    public SubscriptionPlan Plan { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime? TrialEndsAt { get; set; }
    public DateTime? CurrentPeriodStartsAt { get; set; }
    public DateTime? CurrentPeriodEndsAt { get; set; }
    public DateTime? CanceledAt { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public string? CancellationReason { get; set; }

    // Usage Limits
    public int ContactLimit { get; set; }
    public int UserLimit { get; set; }
    public int StorageLimit { get; set; } // In MB
    public int MonthlyEmailLimit { get; set; }
    public int MonthlySMSLimit { get; set; }
    public int MonthlyWhatsAppLimit { get; set; }
    public int MonthlyAITokenLimit { get; set; }
    public int AdditionalAITokens { get; set; } // Purchased extra tokens

    // Current Usage
    public int CurrentContacts { get; set; }
    public int CurrentUsers { get; set; }
    public int CurrentStorage { get; set; }
    public int CurrentMonthEmailCount { get; set; }
    public int CurrentMonthSMSCount { get; set; }
    public int CurrentMonthWhatsAppCount { get; set; }
    public int CurrentMonthAITokenCount { get; set; }
    public DateTime? LastUsageResetDate { get; set; }

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
