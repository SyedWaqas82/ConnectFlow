using System.Reflection;
using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior that validates tenant limits for entities with detailed error information.
/// </summary>
public class ValidateLimitsBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ISubscriptionManagementService _subscriptionManagementService;
    private readonly IContextManager _contextManager;

    public ValidateLimitsBehaviour(ISubscriptionManagementService subscriptionManagementService, IContextManager contextManager)
    {
        _subscriptionManagementService = subscriptionManagementService;
        _contextManager = contextManager;
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

        foreach (var limitValidationType in attribute.LimitValidationTypes.Distinct())
        {
            var validationResult = await _subscriptionManagementService.ValidateOperationAsync(limitValidationType, 1, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new EntityLimitExceededException(
                    limitValidationType,
                    validationResult.CurrentUsage ?? 0,
                    validationResult.AllowedLimit ?? 0,
                    validationResult.UpgradeRecommendation,
                    validationResult.RecommendedPlan,
                    validationResult.RecommendedPlanPrice);
            }
        }

        return await next();
    }
}