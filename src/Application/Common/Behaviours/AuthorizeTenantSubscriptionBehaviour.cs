using System.Reflection;
using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Security;

namespace ConnectFlow.Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior that checks if the current tenant has an active subscription when required.
/// </summary>
public class AuthorizeTenantSubscriptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IContextValidationService _contextValidationService;

    public AuthorizeTenantSubscriptionBehaviour(IContextValidationService contextValidationService)
    {
        _contextValidationService = contextValidationService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attribute = request.GetType().GetCustomAttribute<AuthorizeTenantSubscriptionAttribute>();

        if (attribute != null)
        {
            var hasActiveSubscription = await _contextValidationService.IsCurrentUserFromCurrentTenantHasActiveSubscriptionAsync(attribute.AllowSuperAdmin);

            if (!hasActiveSubscription)
                throw new SubscriptionRequiredException("This operation requires an active subscription.");

            if (attribute.Roles != null)
            {
                var roles = attribute.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var authorized = await AuthorizeAsync(roles, attribute.AllowSuperAdmin);

                if (!authorized)
                    throw new ForbiddenAccessException();
            }
        }

        return await next();
    }

    private async Task<bool> AuthorizeAsync(string[] roles, bool allowSuperAdmin)
    {
        foreach (var role in roles)
        {
            if (await _contextValidationService.IsCurrentUserFromCurrentTenantHasRoleAsync(role, allowSuperAdmin))
                return true;
        }

        return false;
    }
}