using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.Commands.CreateSubscription;

[AuthorizeTenantSubscription(false, Roles.TenantAdmin)]
public record CreateSubscriptionCommand : IRequest<CreateSubscriptionResult>
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