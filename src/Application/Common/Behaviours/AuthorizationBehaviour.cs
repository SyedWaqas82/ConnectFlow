using System.Reflection;
using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Security;

namespace ConnectFlow.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IContextManager _contextManager;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(IContextManager contextManager, IIdentityService identityService)
    {
        _contextManager = contextManager;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            // Must be authenticated user
            if (_contextManager.GetCurrentApplicationUserPublicId().HasValue == false)
            {
                throw new UnauthorizedAccessException();
            }

            // Role-based authorization
            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = _contextManager.GetCurrentUserRoles().Contains(role.Trim());
                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                // Must be a member of at least one role in roles
                if (!authorized)
                {
                    throw new ForbiddenAccessException("User does not have the required role to access this resource");
                }
            }

            // Policy-based authorization
            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
            var currentApplicationUserPublicId = _contextManager.GetCurrentApplicationUserPublicId();

            if (authorizeAttributesWithPolicies.Any() && currentApplicationUserPublicId.HasValue)
            {
                foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                {
                    var authorized = await _identityService.AuthorizeAsync(currentApplicationUserPublicId.Value, policy);

                    if (!authorized)
                    {
                        throw new ForbiddenAccessException("User is not authorized to access this resource");
                    }
                }
            }
        }

        // User is authorized / authorization not required
        return await next();
    }
}