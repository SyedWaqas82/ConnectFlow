using ConnectFlow.Application.Common.Models;
using ConnectFlow.Infrastructure.Common.HealthChecks;
using ConnectFlow.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace ConnectFlow.Infrastructure.Common.Configuration;

public static class HealthChecksConfiguration
{
    /// <summary>
    /// Adds enhanced health checks for the application and its dependencies
    /// </summary>
    /// <remarks>
    /// Provides health endpoints for:
    /// - Basic health: /health
    /// - Liveness probe: /health/live
    /// - Readiness probe: /health/ready
    /// - Dashboard UI: /healthz
    /// 
    /// For complete documentation on health checks, configuration options, and 
    /// dashboard links, please see monitoring/README.md
    /// </remarks>
    public static void AddEnhancedHealthChecks(this IHostApplicationBuilder builder)
    {
        // Get health check settings from configuration
        var healthCheckSettings = builder.Configuration.GetSection("HealthChecksUI").Get<HealthChecksSettings>() ?? new HealthChecksSettings();

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("Database",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "database", "ready" })
            .AddNpgSql(
                builder.Configuration.GetConnectionString("ConnectFlowDb") ??
                    builder.Configuration.GetConnectionString("DefaultConnection")!,
                name: "postgres",
                tags: new[] { "database", "postgres", "ready" })
            .AddRedis(
                builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379",
                name: "redis",
                tags: new[] { "cache", "redis", "ready" })
            .AddCheck<QuartzHealthCheck>(
                "quartz",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "jobs", "quartz", "ready" })
            .AddCheck("API", () => HealthCheckResult.Healthy(),
                tags: new[] { "service", "live" });

        // Add Health Checks UI
        builder.Services.AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(healthCheckSettings.EvaluationTimeInSeconds);
            options.MaximumHistoryEntriesPerEndpoint(50);
            options.SetApiMaxActiveRequests(1);
            options.SetMinimumSecondsBetweenFailureNotifications(healthCheckSettings.MinimumSecondsBetweenFailureNotifications);
        }).AddInMemoryStorage();

        // Configure settings in DI container for use elsewhere
        builder.Services.Configure<HealthChecksSettings>(builder.Configuration.GetSection("HealthChecksUI"));
    }
}