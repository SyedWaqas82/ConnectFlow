using System.Reflection;
using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Security;

namespace ConnectFlow.Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior that validates tenant limits for entities.
/// </summary>
public class ValidateLimitsBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IContextValidationService _contextValidationService;
    private readonly IContextManager _contextManager;

    public ValidateLimitsBehaviour(IContextValidationService contextValidationService, IContextManager contextManager)
    {
        _contextValidationService = contextValidationService;
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
            if (!await _contextValidationService.CanAddEntityAsync(limitValidationType))
            {
                throw new EntityLimitExceededException(
                    $"You have reached the limit for {limitValidationType} entities in your current subscription plan.");
            }
        }

        return await next();
    }
}