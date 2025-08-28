using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.Commands.CancelSubscription;

[AuthorizeTenant(true, true, Roles.TenantAdmin)]
public record CancelSubscriptionCommand : IRequest<CancelSubscriptionResult>
{
    public bool CancelImmediately { get; init; } = false;
}

public record CancelSubscriptionResult
{
    public int SubscriptionId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTimeOffset? CancelledAt { get; init; }
    public DateTimeOffset? EffectiveDate { get; init; }
}