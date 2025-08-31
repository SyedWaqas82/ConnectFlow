using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.Commands.UpdateSubscription;

[AuthorizeTenant(false, true, Roles.TenantAdmin)]
public record UpdateSubscriptionCommand : IRequest<Result<UpdateSubscriptionResult>>
{
    public int NewPlanId { get; init; }
}

public record UpdateSubscriptionResult
{
    public int SubscriptionId { get; init; }
    public string Status { get; init; } = string.Empty;
}