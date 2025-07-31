using Asp.Versioning;
using Azure.Identity;
using ConnectFlow.Web.Common;
using Microsoft.AspNetCore.Mvc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();
        //builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
        builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }

    /// <summary>
    /// Adds API versioning services to the specified <see cref="WebApplicationBuilder"/>.
    /// </summary>
    public static WebApplicationBuilder AddApiVersioning(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                // Format the version as "v1" in the Swagger UI
                options.GroupNameFormat = "'v'VVV";
                // Replace version in the URL path
                options.SubstituteApiVersionInUrl = true;
                // This is important for Swagger to discover all versions
                options.AssumeDefaultVersionWhenUnspecified = true;
            });

        return builder;
    }

    public static void UseCustomHealthChecks(this WebApplication app)
    {
        // Configure health check endpoints
        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = reg => reg.Tags.Contains("live"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = reg => reg.Tags.Contains("ready"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Add Health Checks UI endpoint
        app.MapHealthChecksUI(options =>
        {
            options.UIPath = "/healthz";
            options.ApiPath = "/healthz-api";
        });
    }
}