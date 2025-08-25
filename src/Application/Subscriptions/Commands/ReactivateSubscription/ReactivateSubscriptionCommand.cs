using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.Commands.ReactivateSubscription;

[AuthorizeTenantSubscription(false, Roles.TenantAdmin)]
public record ReactivateSubscriptionCommand : IRequest<ReactivateSubscriptionResult>;

public record ReactivateSubscriptionResult
{
    public int SubscriptionId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}