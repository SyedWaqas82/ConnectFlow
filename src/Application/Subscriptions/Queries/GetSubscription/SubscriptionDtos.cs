namespace ConnectFlow.Application.Subscriptions.Queries.GetSubscription;

public record SubscriptionDto
{
    public int Id { get; init; }
    public string Status { get; init; } = string.Empty;
    public string PlanName { get; init; } = string.Empty;
    public string PlanType { get; init; } = string.Empty;
    public decimal PlanPrice { get; init; }
    public string BillingCycle { get; init; } = string.Empty;
    public DateTimeOffset CurrentPeriodStart { get; init; }
    public DateTimeOffset CurrentPeriodEnd { get; init; }
    public bool CancelAtPeriodEnd { get; init; }
    public DateTimeOffset? CanceledAt { get; init; }

    public PlanLimitsDto Limits { get; init; } = new();
    public UsageDto Usage { get; init; } = new();
}

public record PlanLimitsDto
{
    public int MaxUsers { get; init; }
    public int MaxChannels { get; init; }
    public int MaxWhatsAppChannels { get; init; }
    public int MaxFacebookChannels { get; init; }
    public int MaxInstagramChannels { get; init; }
    public int MaxTelegramChannels { get; init; }
}

public record UsageDto
{
    public int Users { get; init; }
    public int TotalChannels { get; init; }
    public int WhatsAppChannels { get; init; }
    public int FacebookChannels { get; init; }
    public int InstagramChannels { get; init; }
    public int TelegramChannels { get; init; }
}

public class SubscriptionProfile : Profile
{
    public SubscriptionProfile()
    {
        CreateMap<Subscription, SubscriptionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name))
            .ForMember(dest => dest.PlanType, opt => opt.MapFrom(src => src.Plan.Type.ToString()))
            .ForMember(dest => dest.PlanPrice, opt => opt.MapFrom(src => src.Plan.Price))
            .ForMember(dest => dest.BillingCycle, opt => opt.MapFrom(src => src.Plan.BillingCycle.ToString()))
            .ForMember(dest => dest.Limits, opt => opt.MapFrom(src => src.Plan))
            .ForMember(dest => dest.Usage, opt => opt.Ignore()); // Usage will be mapped separately from usage statistics

        CreateMap<Plan, PlanLimitsDto>();

        // Create a custom mapping for usage statistics dictionary to UsageDto
        CreateMap<Dictionary<string, int>, UsageDto>()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.GetValueOrDefault("Users", 0)))
            .ForMember(dest => dest.TotalChannels, opt => opt.MapFrom(src => src.GetValueOrDefault("TotalChannels", 0)))
            .ForMember(dest => dest.WhatsAppChannels, opt => opt.MapFrom(src => src.GetValueOrDefault("WhatsAppChannels", 0)))
            .ForMember(dest => dest.FacebookChannels, opt => opt.MapFrom(src => src.GetValueOrDefault("FacebookChannels", 0)))
            .ForMember(dest => dest.InstagramChannels, opt => opt.MapFrom(src => src.GetValueOrDefault("InstagramChannels", 0)))
            .ForMember(dest => dest.TelegramChannels, opt => opt.MapFrom(src => src.GetValueOrDefault("TelegramChannels", 0)));

        // // Additional mappings for other subscription-related DTOs
        // CreateMap<Subscription, SubscriptionDto>()
        //     .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
        //     .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name));

        // CreateMap<Plan, PlanDto>()
        //     .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
        //     .ForMember(dest => dest.BillingCycle, opt => opt.MapFrom(src => src.BillingCycle.ToString()));
    }
}