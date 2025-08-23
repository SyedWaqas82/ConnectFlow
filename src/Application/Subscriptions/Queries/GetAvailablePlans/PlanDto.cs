namespace ConnectFlow.Application.Subscriptions.Queries.GetAvailablePlans;

public record PlanDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PaymentProviderPriceId { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Type { get; init; } = string.Empty;
    public string BillingCycle { get; init; } = string.Empty;
    public int MaxUsers { get; init; }
    public int MaxChannels { get; init; }
    public int MaxWhatsAppChannels { get; init; }
    public int MaxFacebookChannels { get; init; }
    public int MaxInstagramChannels { get; init; }
    public int MaxTelegramChannels { get; init; }
    public bool IsActive { get; init; }
    public bool IsCurrentPlan { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Plan, PlanDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.BillingCycle, opt => opt.MapFrom(src => src.BillingCycle.ToString()));
        }
    }
}