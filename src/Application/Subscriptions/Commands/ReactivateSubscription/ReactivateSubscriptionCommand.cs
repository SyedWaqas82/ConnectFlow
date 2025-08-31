using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.Commands.ReactivateSubscription;

[AuthorizeTenant(false, true, Roles.TenantAdmin)]
public record ReactivateSubscriptionCommand : IRequest<Result<ReactivateSubscriptionResult>>;

public record ReactivateSubscriptionResult
{
    public int SubscriptionId { get; init; }
    public string Status { get; init; } = string.Empty;
}