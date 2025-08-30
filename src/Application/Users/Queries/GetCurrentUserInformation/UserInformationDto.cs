namespace ConnectFlow.Application.Users.Queries.GetCurrentUserInformation;

public record UserInformationDto
{
    public Guid ApplicationUserPublicId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public IList<TenantDto> Tenants { get; init; } = new List<TenantDto>();
}

public record TenantDto
{
    public TenantDto()
    {
        Roles = Array.Empty<string>();
        Subscriptions = Array.Empty<SubscriptionDto>();
    }

    public int Id { get; init; }
    public Guid PublicId { get; init; }
    public string PaymentProviderCustomerId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public IReadOnlyCollection<string> Roles { get; init; }
    public IReadOnlyCollection<SubscriptionDto> Subscriptions { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Tenant, TenantDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.TenantUsers.SelectMany(u => u.TenantUserRoles).Select(r => r.RoleName)))
                .ForMember(dest => dest.Subscriptions, opt => opt.MapFrom(src => src.Subscriptions));
        }
    }
}

public record SubscriptionDto
{
    public int Id { get; init; }
    public Guid PublicId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CurrentPeriodStart { get; init; }
    public DateTimeOffset CurrentPeriodEnd { get; init; }
    public PlanDto Plan { get; init; } = new PlanDto();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(dest => dest.Plan, opt => opt.MapFrom(src => src.Plan))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}

public record PlanDto
{
    public int Id { get; init; }
    public Guid PublicId { get; init; }
    public string Name { get; init; } = string.Empty;
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