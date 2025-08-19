// using ConnectFlow.Application.Common.Interfaces;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;

// namespace ConnectFlow.Web.Infrastructure.Filters;

// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
// public class RequiresTenantRoleAttribute : Attribute, IAsyncAuthorizationFilter
// {
//     public string Roles { get; }
//     public bool AllowSuperAdmin { get; }

//     public RequiresTenantRoleAttribute(string roles, bool allowSuperAdmin = true)
//     {
//         Roles = roles ?? string.Empty;
//         AllowSuperAdmin = allowSuperAdmin;
//     }

//     public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
//     {
//         if (string.IsNullOrWhiteSpace(Roles))
//             return;

//         var contextValidationService = context.HttpContext.RequestServices.GetRequiredService<IContextValidationService>();
//         var roles = Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

//         foreach (var role in roles)
//         {
//             if (await contextValidationService.IsCurrentUserFromCurrentTenantHasRoleAsync(role, AllowSuperAdmin))
//                 return; // Authorized
//         }

//         context.Result = new ForbidResult();
//     }
// }