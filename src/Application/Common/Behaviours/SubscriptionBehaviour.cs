using System.Reflection;
using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Security;
using ConnectFlow.Application.Common.Services;

namespace ConnectFlow.Application.Common.Behaviours;

/// <summary>
/// Pipeline behavior that checks if the current tenant has an active subscription when required
/// </summary>
public class SubscriptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionBehaviour(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requiresSubscriptionAttribute = request.GetType().GetCustomAttribute<SubscriptionAttribute>();

        if (requiresSubscriptionAttribute != null)
        {
            // Get current tenant from TenantInfo
            var currentTenantId = TenantInfo.CurrentTenantId;

            // If no tenant context or user is SuperAdmin, bypass check
            if (!currentTenantId.HasValue || TenantInfo.IsSuperAdmin)
            {
                return await next();
            }

            // Check if tenant has active subscription
            bool hasActiveSubscription = await _subscriptionService.HasActiveSubscriptionAsync(currentTenantId.Value);

            if (!hasActiveSubscription)
            {
                throw new SubscriptionRequiredException($"This operation requires an active subscription.");
            }
        }

        // Subscription check passed or not required
        return await next();
    }
}