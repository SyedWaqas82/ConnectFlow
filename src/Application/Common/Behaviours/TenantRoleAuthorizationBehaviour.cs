using System.Reflection;
using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Security;

namespace ConnectFlow.Application.Common.Behaviours;

public class TenantRoleAuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IContextValidationService _contextValidationService;

    public TenantRoleAuthorizationBehaviour(IContextValidationService contextValidationService)
    {
        _contextValidationService = contextValidationService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attribute = request.GetType().GetCustomAttribute<TenantRoleAttribute>();

        if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Roles))
        {
            var roles = attribute.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var authorized = await AuthorizeAsync(roles, attribute.AllowSuperAdmin);

            if (!authorized)
                throw new ForbiddenAccessException();
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