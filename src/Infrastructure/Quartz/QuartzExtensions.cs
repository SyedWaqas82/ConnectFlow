using ConnectFlow.Infrastructure.Quartz.Jobs;
using Quartz;

namespace ConnectFlow.Infrastructure.Quartz;

/// <summary>
/// Simple extension methods for scheduling jobs with tenant/user context
/// </summary>
public static class QuartzExtensions
{
    /// <summary>
    /// Schedule a job with tenant and user context
    /// </summary>
    public static async Task<DateTimeOffset> ScheduleJobWithContextAsync<T>(this IScheduler scheduler, string jobName, string description, int tenantId = 0, int applicationUserId = 0, string? correlationId = null, string? cronExpression = null, TimeSpan? delay = null, JobDataMap? additionalData = null, bool replaceExisting = false) where T : BaseJob
    {
        var jobKey = new JobKey(jobName);

        // Check if job already exists and handle accordingly
        if (await scheduler.CheckExists(jobKey))
        {
            if (!replaceExisting)
            {
                throw new InvalidOperationException($"Job with key '{jobKey}' already exists. Set replaceExisting=true to replace it.");
            }

            // Delete existing job (this will also delete associated triggers)
            await scheduler.DeleteJob(jobKey);
        }

        // Generate correlation ID if not provided
        correlationId ??= Guid.NewGuid().ToString("N")[..16];

        // Create job with context data (convert to strings for useProperties mode)
        var jobBuilder = JobBuilder.Create<T>()
            .WithIdentity(jobKey)
            .WithDescription(description)
            .UsingJobData(BaseJob.TenantIdKey, tenantId.ToString())
            .UsingJobData(BaseJob.ApplicationUserIdKey, applicationUserId.ToString())
            .UsingJobData(BaseJob.CorrelationIdKey, correlationId);

        // Add any additional data
        if (additionalData != null)
        {
            foreach (var kvp in additionalData)
            {
                jobBuilder = jobBuilder.UsingJobData(kvp.Key, kvp.Value?.ToString());
            }
        }

        var job = jobBuilder.Build();

        // Create trigger
        ITrigger trigger;
        if (!string.IsNullOrEmpty(cronExpression))
        {
            // Cron trigger
            trigger = TriggerBuilder.Create()
                .WithIdentity($"Trigger_{jobName}_{Guid.NewGuid():N}")
                .WithCronSchedule(cronExpression)
                .Build();
        }
        else if (delay.HasValue)
        {
            // Delayed trigger
            trigger = TriggerBuilder.Create()
                .WithIdentity($"Trigger_{jobName}_{Guid.NewGuid():N}")
                .StartAt(DateTimeOffset.UtcNow.Add(delay.Value))
                .Build();
        }
        else
        {
            // Immediate trigger
            trigger = TriggerBuilder.Create()
                .WithIdentity($"Trigger_{jobName}_{Guid.NewGuid():N}")
                .StartNow()
                .Build();
        }

        return await scheduler.ScheduleJob(job, trigger);
    }

    /// <summary>
    /// Schedule a job immediately with tenant context (uses default admin user)
    /// </summary>
    public static Task<DateTimeOffset> ScheduleTenantJobAsync<T>(this IScheduler scheduler, string jobName, string description, int tenantId, string? correlationId = null, JobDataMap? additionalData = null) where T : BaseJob
    {
        return scheduler.ScheduleJobWithContextAsync<T>(jobName, description, tenantId: tenantId, applicationUserId: 0, correlationId: correlationId, additionalData: additionalData);
    }

    /// <summary>
    /// Schedule a system job (no specific tenant/user context)
    /// </summary>
    public static Task<DateTimeOffset> ScheduleSystemJobAsync<T>(this IScheduler scheduler, string jobName, string description, string? correlationId = null, JobDataMap? additionalData = null) where T : BaseJob
    {
        return scheduler.ScheduleJobWithContextAsync<T>(jobName, description, tenantId: 0, applicationUserId: 0, correlationId: correlationId, additionalData: additionalData);
    }

    /// <summary>
    /// Check if a job exists and optionally get its details
    /// </summary>
    public static async Task<bool> JobExistsAsync(this IScheduler scheduler, string jobName)
    {
        var jobKey = new JobKey(jobName);
        return await scheduler.CheckExists(jobKey);
    }

    /// <summary>
    /// Delete a job if it exists
    /// </summary>
    public static async Task<bool> DeleteJobIfExistsAsync(this IScheduler scheduler, string jobName)
    {
        var jobKey = new JobKey(jobName);
        if (await scheduler.CheckExists(jobKey))
        {
            return await scheduler.DeleteJob(jobKey);
        }
        return false;
    }
}