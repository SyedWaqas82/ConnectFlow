namespace ConnectFlow.Application.Common.Models;

public class SubscriptionSettings
{
    public const string SectionName = "SubscriptionSettings";

    public int GracePeriodDays { get; set; } = 7; // Grace period configuration
    public int MaxPaymentRetries { get; set; } = 5; // Stripe's default retry count
    public string DefaultDowngradePlanName { get; set; } = "Free"; // Auto-downgrade configuration
    public int EventIdempotencyCacheDuration { get; set; } = 24; // Cache and event settings In hours
    public bool AllowImmediateCancellations { get; set; } = false; // Allow users to cancel immediately
    public bool AllowDowngrades { get; set; } = false; // Allow users to request downgrades
}