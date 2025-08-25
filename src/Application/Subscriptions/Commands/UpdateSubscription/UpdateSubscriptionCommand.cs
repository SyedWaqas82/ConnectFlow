using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.Commands.UpdateSubscription;

[AuthorizeTenantSubscription(false, Roles.TenantAdmin)]
public record UpdateSubscriptionCommand : IRequest<UpdateSubscriptionResult>
{
    public int NewPlanId { get; init; }
}

public record UpdateSubscriptionResult
{
    public int SubscriptionId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}