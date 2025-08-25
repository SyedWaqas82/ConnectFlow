namespace ConnectFlow.Application.Common.Models;

public class SubscriptionSettings
{
    public const string SectionName = "SubscriptionSettings";

    public int GracePeriodDays { get; set; } = 7;
    public string DefaultDowngradePlanName { get; set; } = "Free";
    public bool AutoDowngradeAfterGracePeriod { get; set; } = true;
    public int EventIdempotencyCacheDuration { get; set; } = 24; // In hours
}