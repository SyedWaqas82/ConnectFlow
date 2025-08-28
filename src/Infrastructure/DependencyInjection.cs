using System.Text;
using ConnectFlow.Application.Common.Models;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Infrastructure.Common.Interfaces;
using ConnectFlow.Infrastructure.Common.Configuration;
using ConnectFlow.Infrastructure.Data;
using ConnectFlow.Infrastructure.Data.Interceptors;
using ConnectFlow.Infrastructure.Identity;
using ConnectFlow.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ConnectFlow.Infrastructure.Common.Models;
using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Infrastructure.Metrics;
using ConnectFlow.Infrastructure.Services.Email;
using ConnectFlow.Infrastructure.Services.Messaging.RabbitMQ;
using ConnectFlow.Domain.Events.Messaging;
using ConnectFlow.Infrastructure.Services.Messaging.RabbitMQ.Consumers;
using ConnectFlow.Application.Common.Messaging.Handlers;
using ConnectFlow.Infrastructure.Services.Payment.Configuration;
using ConnectFlow.Infrastructure.Services.Payment;
using ConnectFlow.Infrastructure.Quartz;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var dbConnectionString = builder.Configuration.GetConnectionString("ConnectFlowDb");
        Guard.Against.Null(dbConnectionString, message: "Connection string 'ConnectFlowDb' not found.");

        var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
        Guard.Against.Null(redisConnectionString, message: "Connection string 'Redis' not found.");

        var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
        Guard.Against.Null(jwtSettings, message: "JWT settings not found in configuration.");
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

        var tenantSettings = builder.Configuration.GetSection(TenantSettings.SectionName).Get<TenantSettings>();
        Guard.Against.Null(tenantSettings, message: "Tenant settings not found in configuration.");
        builder.Services.Configure<TenantSettings>(builder.Configuration.GetSection(TenantSettings.SectionName));

        var redisSettings = builder.Configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>();
        Guard.Against.Null(redisSettings, message: "Redis settings not found in configuration.");
        builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(RedisSettings.SectionName));

        // Email settings
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));

        // Stripe settings
        var stripeSettings = builder.Configuration.GetSection(StripeSettings.SectionName).Get<StripeSettings>();
        Guard.Against.Null(stripeSettings, message: "Stripe settings not found in configuration.");
        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection(StripeSettings.SectionName));

        // Configure subscription settings
        var subscriptionSettings = builder.Configuration.GetSection(SubscriptionSettings.SectionName).Get<SubscriptionSettings>();
        Guard.Against.Null(subscriptionSettings, message: "Subscription settings not found in configuration.");
        builder.Services.Configure<SubscriptionSettings>(builder.Configuration.GetSection(SubscriptionSettings.SectionName));

        // Configure Entity Framework
        builder.ConfigureEntityFramework(dbConnectionString);

        // Configure Identity
        builder.ConfigureIdentity();

        // Configure Authentication and Authorization
        builder.ConfigureAuthenticationAndAuthorization(jwtSettings);

        // Configure Redis
        builder.ConfigureOtherServices(redisConnectionString, redisSettings);

        // Configure RabbitMQ
        builder.AddRabbitMqServices();

        // Add custom services - order matters to avoid circular dependencies
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddScoped<ICacheService, RedisCacheService>();
        builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();

        // Configure email services
        // This should be after RabbitMQ and Identity services to ensure all dependencies are available
        builder.AddEmailServices();

        // Configure context services
        builder.AddContextServices();

        // Register payment and subscription services
        builder.Services.AddScoped<IPaymentService, StripeService>();
        builder.Services.AddScoped<ISubscriptionManagementService, SubscriptionManagementService>();
        builder.Services.AddScoped<IPaymentWebhookEventHandlerService, StripeWebhookEventHandlerService>();

        // Register IIdentityService after all dependencies are registered
        builder.Services.AddTransient<IIdentityService, IdentityService>();
    }

    private static void ConfigureEntityFramework(this IHostApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, TenantFilterInterceptor>();

        // Add Database Context and interceptors
        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString).AddAsyncSeeding(sp);
        });

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        builder.Services.AddScoped<ApplicationDbContextInitialiser>();
    }

    private static void ConfigureIdentity(this IHostApplicationBuilder builder)
    {
        // Add and Configure Identity services
        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        //Configure Identity
        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 7;
            options.Lockout.AllowedForNewUsers = true;

            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // SignIn settings.
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            // User settings.
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
        });
    }

    private static void ConfigureAuthenticationAndAuthorization(this IHostApplicationBuilder builder, JwtSettings jwtSettings)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            };
        });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.SuperAdmin));
        //services.AddAuthorization(options => options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));
    }

    private static void ConfigureOtherServices(this IHostApplicationBuilder builder, string redisConnectionString, RedisSettings redisSettings)
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = redisSettings.InstanceName;
        });

        // Configure Serilog first, before any other services to ensure proper logging
        builder.AddStructuredLogging();

        // Register background job services with Quartz
        builder.AddQuartzInfrastructure();

        // Add observability services (metrics, tracing)
        builder.AddOpenTelemetry();

        // Add rate limiting services
        builder.AddRateLimiting();

        // Add health checks
        builder.AddEnhancedHealthChecks();
    }

    private static void AddRabbitMqServices(this IHostApplicationBuilder builder)
    {
        // Configure RabbitMQ settings
        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection(RabbitMQSettings.SectionName));

        // Core RabbitMQ services
        builder.Services.AddSingleton<IRabbitMQConnectionManager, RabbitMQConnectionManager>();
        builder.Services.AddSingleton<IRabbitMQSetupService, RabbitMQSetupService>();
        builder.Services.AddScoped<IMessagePublisher, RabbitMQPublisher>();

        // Add RabbitMQ metrics
        builder.Services.AddSingleton<RabbitMQMetrics>();

        // Message handlers
        builder.Services.AddScoped<IMessageHandler<EmailSendMessageEvent>, EmailSendMessageEventHandler>();

        // Consumers as hosted services
        builder.Services.AddHostedService<EmailConsumer>();

        // Setup service
        builder.Services.AddHostedService<RabbitMQSetupHostedService>();

        // Register all message handlers from the assembly
        var assembly = typeof(EmailSendMessageEventHandler).Assembly;

        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IMessageHandler<>)))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var interfaceType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>));

            builder.Services.AddScoped(interfaceType, handlerType);
        }
    }

    private static void AddEmailServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<EmailMetrics>();

        builder.Services.AddSingleton<IEmailTemplateRenderer, RazorEmailTemplateRenderer>();
        builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
        builder.Services.AddTransient<IEmailService, EmailService>();
    }

    private static void AddContextServices(this IHostApplicationBuilder builder)
    {
        // Register HTTP context accessor for context services
        builder.Services.AddHttpContextAccessor();

        // Register unified context service with all interfaces
        builder.Services.AddScoped<UnifiedContextService>();
        builder.Services.AddScoped<IContextManager>(sp => sp.GetRequiredService<UnifiedContextService>());
    }
}