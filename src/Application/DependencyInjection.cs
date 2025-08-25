using System.Reflection;
using ConnectFlow.Application.Common.Behaviours;
using ConnectFlow.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            // Add subscription and entity limit behaviors
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizeTenantSubscriptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidateLimitsBehaviour<,>));
        });

        var subscriptionSettings = builder.Configuration.GetSection(SubscriptionSettings.SectionName).Get<SubscriptionSettings>();
        Guard.Against.Null(subscriptionSettings, message: "Subscription settings not found in configuration.");
        builder.Services.Configure<SubscriptionSettings>(builder.Configuration.GetSection(SubscriptionSettings.SectionName));
    }
}