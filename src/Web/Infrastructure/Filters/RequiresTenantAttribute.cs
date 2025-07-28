using ConnectFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConnectFlow.Web.Infrastructure.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequiresActiveSubscriptionAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var contextService = context.HttpContext.RequestServices.GetRequiredService<IContextService>();
        var subscriptionService = context.HttpContext.RequestServices.GetRequiredService<ISubscriptionService>();

        var tenantId = contextService.GetCurrentTenantId();

        if (!tenantId.HasValue)
        {
            context.Result = new ForbidResult();
            return;
        }

        bool hasActiveSubscription = await subscriptionService.HasActiveSubscriptionAsync(tenantId.Value);

        if (!hasActiveSubscription)
        {
            context.Result = new ObjectResult(new
            {
                Error = "Subscription Required",
                Message = "This operation requires an active subscription."
            })
            {
                StatusCode = 402 // Payment Required
            };
            return;
        }
    }
}