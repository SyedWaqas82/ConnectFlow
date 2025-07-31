﻿using System.Text;
using ConnectFlow.Application.Common.Interfaces;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("ConnectFlowDb");
        Guard.Against.Null(connectionString, message: "Connection string 'ConnectFlowDb' not found.");

        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        Guard.Against.Null(jwtSettings, message: "JWT settings not found in configuration.");
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        var tenantSettings = builder.Configuration.GetSection("TenantSettings").Get<TenantSettings>();
        Guard.Against.Null(tenantSettings, message: "Tenant settings not found in configuration.");
        builder.Services.Configure<TenantSettings>(builder.Configuration.GetSection("TenantSettings"));

        var redisSettings = builder.Configuration.GetSection("RedisSettings").Get<RedisSettings>();
        Guard.Against.Null(redisSettings, message: "Redis settings not found in configuration.");
        builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));

        // Configure Entity Framework
        builder.ConfigureEntityFramework(connectionString);

        // Configure Identity
        builder.ConfigureIdentity();

        // Configure Authentication and Authorization
        builder.ConfigureAuthenticationAndAuthorization(jwtSettings);

        // Configure Redis
        builder.ConfigureOtherServices(redisSettings);

        // Add custom services - order matters to avoid circular dependencies
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddScoped<ICacheService, RedisCacheService>();
        builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();
        builder.Services.AddScoped<IStripeService, StripeService>();

        // Register IContextService last as it might depend on multiple services
        builder.Services.AddScoped<IContextService, ContextService>();

        // Register IContextValidationService before services that depend on it
        builder.Services.AddScoped<IContextValidationService, ContextValidationService>();

        // Register ITenantLimitsService after IContextValidationService
        builder.Services.AddScoped<ITenantLimitsService, TenantLimitsService>();

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

    private static void ConfigureOtherServices(this IHostApplicationBuilder builder, RedisSettings redisSettings)
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisSettings.Configuration;
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
}