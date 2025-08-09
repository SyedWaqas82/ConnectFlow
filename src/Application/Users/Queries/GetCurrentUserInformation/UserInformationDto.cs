using ConnectFlow.Domain.Entities;
using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Application.Users.Queries.GetCurrentUserInformation;

public class UserInformationDto
{
    public Guid UserId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public IList<TenantDto> Tenants { get; init; } = new List<TenantDto>();
}

public class TenantDto
{
    public TenantDto()
    {
        Roles = Array.Empty<string>();
        Subscriptions = Array.Empty<SubscriptionDto>();
    }

    public int Id { get; init; }
    public Guid PublicId { get; init; }
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

public class SubscriptionDto
{
    public int Id { get; init; }
    public Guid PublicId { get; init; }
    public SubscriptionStatus Status { get; init; }
    public BillingCycle BillingCycle { get; init; }
    public DateTimeOffset CurrentPeriodStartsAt { get; init; }
    public DateTimeOffset CurrentPeriodEndsAt { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Subscription, SubscriptionDto>();
        }
    }
}