using System.Reflection;
using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Security;

namespace ConnectFlow.Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior that validates tenant limits for entities with detailed error information.
/// </summary>
public class ValidateLimitsBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IContextManager _contextManager;
    private readonly ISubscriptionManagementService _subscriptionManagementService;

    public ValidateLimitsBehaviour(IContextManager contextManager, ISubscriptionManagementService subscriptionManagementService)
    {
        _contextManager = contextManager;
        _subscriptionManagementService = subscriptionManagementService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attribute = request.GetType().GetCustomAttribute<ValidateLimitsAttribute>();

        if (attribute == null)
            return await next();

        var tenantId = _contextManager.GetCurrentTenantId();

        // Bypass check if no tenant context or user is SuperAdmin
        if (!tenantId.HasValue || _contextManager.IsSuperAdmin())
            return await next();

        foreach (var entityType in attribute.LimitValidationTypes.Distinct())
        {
            var count = await _subscriptionManagementService.CanAddEntityAsync(entityType, cancellationToken);
            if (!count.CanAdd)
            {
                throw new SubscriptionLimitExceededException(entityType.ToString(), count.MaxCount, count.CurrentCount);
            }
        }

        return await next();
    }
}