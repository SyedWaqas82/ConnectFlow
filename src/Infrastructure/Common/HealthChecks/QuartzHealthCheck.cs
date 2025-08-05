using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;

namespace ConnectFlow.Infrastructure.Common.HealthChecks;

/// <summary>
/// Health check for Quartz.NET scheduler
/// </summary>
public class QuartzHealthCheck : IHealthCheck
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ILogger<QuartzHealthCheck> _logger;

    public QuartzHealthCheck(ISchedulerFactory schedulerFactory, ILogger<QuartzHealthCheck> logger)
    {
        _schedulerFactory = schedulerFactory;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            if (scheduler.IsShutdown)
            {
                return HealthCheckResult.Unhealthy("Quartz scheduler is shut down");
            }

            if (!scheduler.IsStarted)
            {
                return HealthCheckResult.Degraded("Quartz scheduler is not started");
            }

            // Get metrics data to include in the health report
            var metricData = new Dictionary<string, object>
            {
                { "SchedulerName", scheduler.SchedulerName },
                { "SchedulerInstanceId", scheduler.SchedulerInstanceId },
                { "IsStarted", scheduler.IsStarted },
                { "InStandbyMode", scheduler.InStandbyMode }
            };

            // If everything is working correctly
            return HealthCheckResult.Healthy("Quartz scheduler is running normally", data: metricData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check Quartz scheduler health");
            return HealthCheckResult.Unhealthy($"Quartz health check failed: {ex.Message}");
        }
    }
}