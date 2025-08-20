namespace ConnectFlow.Infrastructure.Services.Payment.Configuration;

public class StripeSettings
{
    public const string SectionName = "StripeSettings";

    public required string SecretKey { get; init; }
    public required string PublishableKey { get; init; }
    public required string WebhookSecret { get; init; }
    public required Dictionary<string, StripePriceConfig> Prices { get; init; }
    public string Currency { get; init; } = "USD";
    public int DefaultTrialDays { get; init; } = 14;
    public bool EnableAutomaticTax { get; init; } = false;
}

public class StripePriceConfig
{
    public required string MonthlyPriceId { get; init; }
    public required string YearlyPriceId { get; init; }
    public required decimal MonthlyAmount { get; init; }
    public required decimal YearlyAmount { get; init; }
    public int TrialDays { get; init; } = 14;
    public List<string> Features { get; init; } = new();
}