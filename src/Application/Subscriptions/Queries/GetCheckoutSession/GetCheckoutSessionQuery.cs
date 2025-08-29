using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.Queries.GetCheckoutSession;

[AuthorizeTenant(false, false, Roles.TenantAdmin)]
public record GetCheckoutSessionQuery : IRequest<CheckoutSessionStatusDto>
{
    public string SessionId { get; init; } = string.Empty;
}

public record CheckoutSessionStatusDto
{
    public string SessionId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string CustomerId { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public bool IsExpired { get; init; }
    public bool IsOpen { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = new();
}