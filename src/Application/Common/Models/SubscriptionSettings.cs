namespace ConnectFlow.Application.Common.Models;

public class SubscriptionSettings
{
    public const string SectionName = "SubscriptionSettings";

    // Grace period configuration - should be longer than Stripe's retry period
    public int GracePeriodDays { get; set; } = 30; // Extended to cover Stripe's full retry cycle

    // Stripe retry configuration
    public int MaxPaymentRetries { get; set; } = 4; // Stripe's default retry count
    public int StripeRetryPeriodDays { get; set; } = 25; // Stripe's maximum retry period

    // Auto-downgrade configuration
    public string DefaultDowngradePlanName { get; set; } = "Free";
    public bool AutoDowngradeAfterGracePeriod { get; set; } = true;
    public bool AutoDowngradeAfterMaxRetries { get; set; } = false; // Option to downgrade after max retries instead of waiting for grace period

    // Cache and event settings
    public int EventIdempotencyCacheDuration { get; set; } = 24; // In hours

    // Advanced retry behavior
    public bool UseIntelligentGracePeriod { get; set; } = true; // Adjust grace period based on retry attempts
    public int RetryAttemptGracePeriodHours { get; set; } = 72; // Additional grace period per retry attempt
}