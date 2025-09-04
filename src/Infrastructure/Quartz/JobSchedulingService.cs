using Microsoft.Extensions.Hosting;
using Quartz;
using ConnectFlow.Infrastructure.Quartz.Jobs;

namespace ConnectFlow.Infrastructure.Quartz;

/// <summary>
/// Service responsible for scheduling jobs using the enhanced BaseJob extensions
/// </summary>
public class JobSchedulingService : IHostedService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JobSchedulingService> _logger;
    private IScheduler? _scheduler;

    public JobSchedulingService(ISchedulerFactory schedulerFactory, IConfiguration configuration, ILogger<JobSchedulingService> logger)
    {
        _schedulerFactory = schedulerFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting enhanced job scheduling with BaseJob extensions");

        try
        {
            // Wait for schema to be ready with retry logic
            await WaitForSchemaReady(cancellationToken);

            // Get the scheduler now that the schema should be initialized
            _scheduler = await _schedulerFactory.GetScheduler();

            // Schedule all jobs using the new enhanced extensions
            await ScheduleEnhancedJobs();

            _logger.LogInformation("Successfully scheduled all enhanced jobs");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule enhanced jobs");
            throw;
        }
    }

    private async Task WaitForSchemaReady(CancellationToken cancellationToken)
    {
        const int maxRetries = 10;
        const int delayMs = 1000;

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                _logger.LogDebug("Attempting to verify Quartz schema readiness (attempt {Attempt}/{MaxRetries})", i + 1, maxRetries);

                // Try to get a scheduler to test if schema is ready
                var testScheduler = await _schedulerFactory.GetScheduler();

                // If we get here without exception, schema is ready
                _logger.LogInformation("Quartz schema verified as ready");
                return;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                _logger.LogWarning("Quartz schema not ready yet (attempt {Attempt}/{MaxRetries}): {Error}. Retrying in {DelayMs}ms...",
                    i + 1, maxRetries, ex.Message, delayMs);

                await Task.Delay(delayMs, cancellationToken);
            }
        }

        // If we get here, all retries failed
        _logger.LogError("Quartz schema failed to become ready after {MaxRetries} attempts", maxRetries);
        throw new InvalidOperationException($"Quartz schema is not ready after {maxRetries} attempts. Please ensure the database is accessible and migrations have run.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Enhanced job scheduling service stopped");
        return Task.CompletedTask;
    }

    private async Task ScheduleEnhancedJobs()
    {
        // DataCleanupJob: runs daily at 2:00 AM (system job)
        await ScheduleJobFromConfig<DataCleanupJob>(defaultCron: "0 0 2 * * ?", description: "Daily data cleanup job");

        // SubscriptionGracePeriodJob: runs every hour (system job)
        await ScheduleJobFromConfig<SubscriptionGracePeriodJob>(defaultCron: "0 0 * * * ?", description: "Hourly subscription grace period check");

        // ReportGenerationJob (Weekly): runs every Monday at 8:00 AM
        await ScheduleJobFromConfig<ReportGenerationJob>(defaultCron: "0 0 8 ? * MON", description: "Weekly report generation job", suffix: "Weekly", additionalData: new JobDataMap { ["ReportType"] = "Weekly" });

        // ReportGenerationJob (Monthly): runs on the 1st of every month at 6:00 AM
        await ScheduleJobFromConfig<ReportGenerationJob>(defaultCron: "0 0 6 1 * ?", description: "Monthly report generation job", suffix: "Monthly", additionalData: new JobDataMap { ["ReportType"] = "Monthly" });

        // TestMetricsJob: runs every minute
        await ScheduleJobFromConfig<TestMetricsJob>(defaultCron: "0 * * * * ?", description: "Test metrics job");

        // LongRunningJob: runs every 3 seconds (for misfire testing)
        //await ScheduleJobFromConfig<LongRunningJob>(defaultCron: "0/3 * * * * ?", description: "Long running job that will cause misfires");
    }

    private async Task ScheduleJobFromConfig<T>(string defaultCron, string description, string? suffix = null, JobDataMap? additionalData = null, int tenantId = 0, int applicationUserId = 0) where T : BaseJob
    {
        var jobName = typeof(T).Name;
        if (!string.IsNullOrEmpty(suffix))
        {
            jobName = $"{jobName}.{suffix}";
        }

        // Get configuration section for this job
        var jobConfig = _configuration.GetSection($"QuartzJobs:{jobName}");

        // Get job parameters from configuration or use defaults
        var cronExpression = jobConfig["CronExpression"] ?? defaultCron;
        var jobDescription = jobConfig["Description"] ?? description;
        var enabled = bool.TryParse(jobConfig["Enabled"], out var parsedEnabled) ? parsedEnabled : true;

        if (!enabled)
        {
            _logger.LogInformation("Job {JobName} is disabled, skipping scheduling", jobName);
            return;
        }

        // Generate correlation ID for this scheduled job
        var correlationId = $"scheduled_{jobName}_{DateTimeOffset.UtcNow:yyyyMMddHHmmss}";

        if (_scheduler == null)
        {
            throw new InvalidOperationException("Scheduler is not initialized. JobSchedulingService must be started before scheduling jobs.");
        }

        try
        {
            // Use the enhanced scheduling extensions
            if (tenantId > 0)
            {
                // Schedule as tenant job
                var scheduledTime = await _scheduler.ScheduleJobWithContextAsync<T>(
                    tenantId: tenantId,
                    applicationUserId: applicationUserId,
                    correlationId: correlationId,
                    cronExpression: cronExpression,
                    additionalData: additionalData
                );

                _logger.LogInformation("Scheduled tenant job {JobName} for tenant {TenantId} at {ScheduledTime} with cron '{CronExpression}'", jobName, tenantId, scheduledTime, cronExpression);
            }
            else
            {
                // Schedule as system job
                var scheduledTime = await _scheduler.ScheduleJobWithContextAsync<T>(
                    tenantId: 0,
                    applicationUserId: 0,
                    correlationId: correlationId,
                    cronExpression: cronExpression,
                    additionalData: additionalData
                );

                _logger.LogInformation("Scheduled system job {JobName} at {ScheduledTime} with cron '{CronExpression}'", jobName, scheduledTime, cronExpression);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule job {JobName} with cron '{CronExpression}'", jobName, cronExpression);
            throw;
        }
    }
}