using ConnectFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConnectFlow.Web.Infrastructure.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequiresTenantAttribute : Attribute, IAsyncAuthorizationFilter
{
    public bool AllowSuperAdmin { get; }

    public RequiresTenantAttribute(bool allowSuperAdmin = true)
    {
        AllowSuperAdmin = allowSuperAdmin;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var contextValidationService = context.HttpContext.RequestServices.GetRequiredService<IContextValidationService>();

        var isValid = await contextValidationService.IsCurrentUserFromCurrentTenantAsync(AllowSuperAdmin);

        if (!isValid)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}