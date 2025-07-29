using System.Reflection;
using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Security;

namespace ConnectFlow.Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior that checks if the current tenant has an active subscription when required.
/// </summary>
public class SubscriptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IContextValidationService _contextValidationService;

    public SubscriptionBehaviour(IContextValidationService contextValidationService)
    {
        _contextValidationService = contextValidationService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attribute = request.GetType().GetCustomAttribute<TenantWithActiveSubscriptionAttribute>();

        if (attribute != null)
        {
            var hasActiveSubscription = await _contextValidationService
                .IsCurrentUserFromCurrentTenantHasActiveSubscriptionAsync(attribute.AllowSuperAdmin);

            if (!hasActiveSubscription)
                throw new SubscriptionRequiredException("This operation requires an active subscription.");
        }

        return await next();
    }
}