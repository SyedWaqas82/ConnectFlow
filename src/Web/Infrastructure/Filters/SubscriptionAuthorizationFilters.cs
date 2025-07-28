// using ConnectFlow.Application.Common.Interfaces;
// using ConnectFlow.Domain.Enums;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;

// namespace ConnectFlow.Web.Infrastructure.Filters;



// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
// public class ValidateEntityLimitAttribute : Attribute, IAsyncActionFilter
// {
//     private readonly EntityType _entityType;

//     public ValidateEntityLimitAttribute(EntityType entityType)
//     {
//         _entityType = entityType;
//     }

//     public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//     {
//         var tenantService = context.HttpContext.RequestServices.GetRequiredService<ITenantService>();
//         var tenantLimitsService = context.HttpContext.RequestServices.GetRequiredService<ITenantLimitsService>();

//         var tenantId = await tenantService.GetCurrentTenantIdAsync();

//         if (tenantId.HasValue)
//         {
//             bool canAddEntity = await tenantLimitsService.CanAddEntityAsync(tenantId.Value, _entityType);

//             if (!canAddEntity)
//             {
//                 context.Result = new ObjectResult(new
//                 {
//                     Error = "Subscription Limit Reached",
//                     Message = $"You have reached the maximum number of {_entityType.ToString().ToLower()}s allowed by your subscription plan."
//                 })
//                 {
//                     StatusCode = 402 // Payment Required
//                 };
//                 return;
//             }
//         }

//         await next();
//     }
// }

// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
// public class ValidateUserLimitAttribute : Attribute, IAsyncActionFilter
// {
//     public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//     {
//         var tenantService = context.HttpContext.RequestServices.GetRequiredService<ITenantService>();
//         var tenantLimitsService = context.HttpContext.RequestServices.GetRequiredService<ITenantLimitsService>();

//         var tenantId = await tenantService.GetCurrentTenantIdAsync();

//         if (tenantId.HasValue)
//         {
//             bool canAddUser = await tenantLimitsService.CanAddUserAsync(tenantId.Value);

//             if (!canAddUser)
//             {
//                 context.Result = new ObjectResult(new
//                 {
//                     Error = "User Limit Reached",
//                     Message = "You have reached the maximum number of users allowed by your subscription plan."
//                 })
//                 {
//                     StatusCode = 402 // Payment Required
//                 };
//                 return;
//             }
//         }

//         await next();
//     }
// }

// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
// public class ValidateAiTokenLimitAttribute : Attribute, IAsyncActionFilter
// {
//     private readonly int _tokenCount;

//     public ValidateAiTokenLimitAttribute(int tokenCount)
//     {
//         _tokenCount = tokenCount;
//     }

//     public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//     {
//         var tenantService = context.HttpContext.RequestServices.GetRequiredService<ITenantService>();
//         var tenantLimitsService = context.HttpContext.RequestServices.GetRequiredService<ITenantLimitsService>();

//         var tenantId = await tenantService.GetCurrentTenantIdAsync();

//         if (tenantId.HasValue)
//         {
//             bool canUseTokens = await tenantLimitsService.CanUseAiTokensAsync(tenantId.Value, _tokenCount);

//             if (!canUseTokens)
//             {
//                 context.Result = new ObjectResult(new
//                 {
//                     Error = "AI Token Limit Reached",
//                     Message = "You have reached the maximum AI token usage allowed by your subscription plan."
//                 })
//                 {
//                     StatusCode = 402 // Payment Required
//                 };
//                 return;
//             }
//         }

//         await next();
//     }
// }
