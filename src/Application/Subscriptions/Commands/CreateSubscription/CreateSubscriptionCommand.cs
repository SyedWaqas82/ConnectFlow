using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.Commands.CreateSubscription;

[AuthorizeTenant(false, false, Roles.TenantAdmin)]
public record CreateSubscriptionCommand : IRequest<Result<CreateSubscriptionResult>>
{
    public int PlanId { get; init; }
    public string SuccessUrl { get; init; } = string.Empty;
    public string CancelUrl { get; init; } = string.Empty;
}

public record CreateSubscriptionResult
{
    public string SessionId { get; init; } = string.Empty;
    public string CheckoutUrl { get; init; } = string.Empty;
}