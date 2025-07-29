using ConnectFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConnectFlow.Web.Infrastructure.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequiresTenantWithActiveSubscriptionAttribute : Attribute, IAsyncAuthorizationFilter
{
    public bool AllowSuperAdmin { get; }

    public RequiresTenantWithActiveSubscriptionAttribute(bool allowSuperAdmin = true)
    {
        AllowSuperAdmin = allowSuperAdmin;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var contextValidationService = context.HttpContext.RequestServices.GetRequiredService<IContextValidationService>();

        var isValid = await contextValidationService.IsCurrentUserFromCurrentTenantHasActiveSubscriptionAsync(AllowSuperAdmin);

        if (!isValid)
        {
            context.Result = new ObjectResult(new
            {
                Error = "Subscription Required",
                Message = "This operation requires an active subscription."
            })
            {
                StatusCode = 402 // Payment Required
            };
        }
    }
}