using System.Reflection;
using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Security;
using ConnectFlow.Application.Common.Services;

namespace ConnectFlow.Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior that validates tenant limits for entities
/// </summary>
public class EntityLimitBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ITenantLimitsService _tenantLimitsService;

    public EntityLimitBehaviour(ITenantLimitsService tenantLimitsService)
    {
        _tenantLimitsService = tenantLimitsService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var entityLimitAttribute = request.GetType().GetCustomAttribute<EntityLimitAttribute>();

        if (entityLimitAttribute != null)
        {
            var currentTenantId = TenantInfo.CurrentTenantId;

            // If no tenant context or user is SuperAdmin, bypass check
            if (!currentTenantId.HasValue || TenantInfo.IsSuperAdmin)
            {
                return await next();
            }

            // Take unique entity types for efficiency
            var uniqueEntityTypes = entityLimitAttribute.EntityTypes.Distinct();

            foreach (var entityType in uniqueEntityTypes)
            {
                bool canAddEntity = await _tenantLimitsService.CanAddEntityAsync(currentTenantId.Value, entityType);

                if (!canAddEntity)
                {
                    throw new EntityLimitExceededException($"You have reached the limit for {entityType} entities in your current subscription plan.");
                }
            }
        }

        // Entity limit check passed or not required
        return await next();
    }
}