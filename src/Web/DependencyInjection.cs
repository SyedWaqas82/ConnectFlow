using Asp.Versioning;
using Azure.Identity;
using ConnectFlow.Web.Common;
using Microsoft.AspNetCore.Mvc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;

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

        // Add Prometheus metrics endpoint for health checks
        app.UseHealthChecks("/metrics/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "text/plain; charset=utf-8";

                // Format each health check as a Prometheus metric
                var sb = new StringBuilder();
                sb.AppendLine("# HELP health_check_status Health check status (0=Unhealthy, 1=Degraded, 2=Healthy)");
                sb.AppendLine("# TYPE health_check_status gauge");

                foreach (var entry in report.Entries)
                {
                    // Convert health status to numeric value
                    var statusValue = entry.Value.Status switch
                    {
                        HealthStatus.Unhealthy => 0,
                        HealthStatus.Degraded => 1,
                        HealthStatus.Healthy => 2,
                        _ => 0
                    };

                    // Format as Prometheus metric with labels
                    sb.AppendLine($"health_check_status{{name=\"{entry.Key}\",tags=\"{string.Join(",", entry.Value.Tags)}\",description=\"{entry.Value.Description?.Replace("\"", "'")}\"}} {statusValue}");

                    // Add duration metric
                    sb.AppendLine($"health_check_duration_seconds{{name=\"{entry.Key}\"}} {entry.Value.Duration.TotalSeconds}");
                }

                // Add overall health status
                sb.AppendLine($"health_status{{status=\"{report.Status}\"}} {(report.Status == HealthStatus.Healthy ? 1 : 0)}");

                await context.Response.WriteAsync(sb.ToString());
            }
        });

        // Add Health Checks UI endpoint
        app.MapHealthChecksUI(options =>
        {
            options.UIPath = "/healthz";
            options.ApiPath = "/healthz-api";
        });
    }
}