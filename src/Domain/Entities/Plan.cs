namespace ConnectFlow.Domain.Entities;

public class Plan : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Currency { get; set; } = "usd";
    public string PaymentProviderProductId { get; set; } = string.Empty;
    public string PaymentProviderPriceId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public PlanType Type { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public int MaxUsers { get; set; }
    public int MaxChannels { get; set; }
    public int MaxWhatsAppChannels { get; set; }
    public int MaxFacebookChannels { get; set; }
    public int MaxInstagramChannels { get; set; }
    public int MaxTelegramChannels { get; set; }
    public bool IsActive { get; set; } = true;

    public IList<Subscription> Subscriptions { get; private set; } = new List<Subscription>();
}