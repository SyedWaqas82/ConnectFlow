using System.Reflection;
using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Security;

namespace ConnectFlow.Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior that checks if the current tenant has an active subscription when required.
/// </summary>
public class AuthorizeTenantSubscriptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IContextManager _contextManager;
    private readonly ISubscriptionManagementService _subscriptionManagementService;

    public AuthorizeTenantSubscriptionBehaviour(IContextManager contextManager, ISubscriptionManagementService subscriptionManagementService)
    {
        _contextManager = contextManager;
        _subscriptionManagementService = subscriptionManagementService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attribute = request.GetType().GetCustomAttribute<AuthorizeTenantAttribute>();

        if (attribute != null)
        {
            // Must be authenticated user
            if (_contextManager.GetCurrentApplicationUserPublicId().HasValue == false)
            {
                throw new UnauthorizedAccessException();
            }

            if (_contextManager.GetCurrentTenantId().HasValue == false)
            {
                throw new TenantNotFoundException();
            }

            var hasActiveSubscription = await _subscriptionManagementService.IsCurrentUserFromCurrentTenantAsync(attribute.AllowSuperAdmin, attribute.CheckActiveSubscription, cancellationToken);

            if (!hasActiveSubscription)
                throw new SubscriptionRequiredException("This operation requires an active subscription.");

            if (attribute.Roles != null && attribute.Roles.Count > 0)
            {
                var authorized = await AuthorizeAsync(attribute.Roles.ToArray(), attribute.AllowSuperAdmin, cancellationToken);

                if (!authorized)
                    throw new ForbiddenAccessException();
            }
        }

        return await next();
    }

    private async Task<bool> AuthorizeAsync(string[] roles, bool allowSuperAdmin, CancellationToken cancellationToken)
    {
        foreach (var role in roles)
        {
            if (await _subscriptionManagementService.IsCurrentUserFromCurrentTenantHasRoleAsync(role, allowSuperAdmin, cancellationToken))
                return true;
        }

        return false;
    }
}