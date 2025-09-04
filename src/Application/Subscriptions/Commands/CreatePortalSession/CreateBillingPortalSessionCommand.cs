using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.Commands.CreatePortalSession;

[AuthorizeTenant(false, false, Roles.TenantAdmin)]
public record CreateBillingPortalSessionCommand : IRequest<Result<CreateBillingPortalSessionResult>>
{
    public string ReturnUrl { get; init; } = string.Empty;
}

public record CreateBillingPortalSessionResult
{
    public string Url { get; init; } = string.Empty;
}