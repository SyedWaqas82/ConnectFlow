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
    private readonly ITenantLimitsService _tenantLimitsService;
    private readonly ICurrentTenantService _tenantService;

    public ValidateLimitsBehaviour(ITenantLimitsService tenantLimitsService, ICurrentTenantService tenantService)
    {
        _tenantLimitsService = tenantLimitsService;
        _tenantService = tenantService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attribute = request.GetType().GetCustomAttribute<ValidateLimitsAttribute>();

        if (attribute == null)
            return await next();

        var tenantId = _tenantService.GetCurrentTenantId();
        var currentUserService = _tenantService as ICurrentUserService;

        // Bypass check if no tenant context or user is SuperAdmin
        if (!tenantId.HasValue || (currentUserService != null && currentUserService.IsSuperAdmin()))
            return await next();

        foreach (var entityType in attribute.EntityTypes.Distinct())
        {
            if (!await _tenantLimitsService.CanAddEntityAsync(tenantId.Value, entityType))
            {
                throw new EntityLimitExceededException(
                    $"You have reached the limit for {entityType} entities in your current subscription plan.");
            }
        }

        return await next();
    }
}