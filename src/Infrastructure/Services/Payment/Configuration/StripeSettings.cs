namespace ConnectFlow.Infrastructure.Services.Payment.Configuration;

public class StripeSettings
{
    public const string SectionName = "StripeSettings";

    public string PublishableKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public int GracePeriodDays { get; set; } = 7;
    public string DefaultDowngradePlanName { get; set; } = "Free";
}