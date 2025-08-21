namespace ConnectFlow.Domain.Entities;

public class Plan : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string StripePriceId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public PlanType Type { get; set; }
    public int MaxUsers { get; set; }
    public int MaxChannels { get; set; }
    public int MaxWhatsAppChannels { get; set; }
    public int MaxFacebookChannels { get; set; }
    public int MaxInstagramChannels { get; set; }
    public int MaxTelegramChannels { get; set; }
    public bool IsActive { get; set; }

    public IList<Feature> Features { get; private set; } = new List<Feature>();
}