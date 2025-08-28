using Quartz;

namespace ConnectFlow.Infrastructure.Quartz.Jobs;

/// <summary>
/// Base class for all Quartz jobs providing common functionality like context management,
/// correlation tracking, and structured logging. 
/// 
/// All metrics are automatically collected by QuartzMetricsProvider through Quartz listeners,
/// eliminating the need for manual metrics collection in job implementations.
/// </summary>
public abstract class BaseJob : IJob
{
    protected readonly Microsoft.Extensions.Logging.ILogger Logger;
    protected readonly IContextManager ContextManager;
    private static readonly ActivitySource ActivitySource = new("ConnectFlow.Infrastructure");

    // Job data map keys for context
    public const string TenantIdKey = "TenantId";
    public const string ApplicationUserIdKey = "ApplicationUserId";
    public const string CorrelationIdKey = "CorrelationId";

    protected BaseJob(Microsoft.Extensions.Logging.ILogger logger, IContextManager contextManager)
    {
        Logger = logger;
        ContextManager = contextManager;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = GetType().Name;
        var jobId = context.FireInstanceId;
        var correlationId = GetCorrelationId(context);
        var stopwatch = Stopwatch.StartNew();

        using var activity = ActivitySource.StartActivity(jobName, ActivityKind.Internal);
        activity?.SetTag("job.id", jobId);
        activity?.SetTag("job.name", jobName);
        activity?.SetTag("correlation.id", correlationId);

        // Add job data properties to activity tags
        AddJobDataToActivity(activity, context);

        // Set up LogContext with all relevant properties
        using var logContextScope = CreateLogContextScope(context, correlationId, jobName, jobId);

        Logger.LogInformation("Starting job {JobName} (ID: {JobId}) with correlation ID {CorrelationId}", jobName, jobId, correlationId);

        try
        {
            // Initialize context if tenant/user IDs are provided
            await InitializeContextAsync(context);

            // Execute the actual job logic
            await ExecuteJobAsync(context);

            stopwatch.Stop();
            var duration = stopwatch.Elapsed.TotalSeconds;

            Logger.LogInformation("Job {JobName} (ID: {JobId}) completed successfully in {Duration:F2}s", jobName, jobId, duration);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var duration = stopwatch.Elapsed.TotalSeconds;

            Logger.LogError(ex, "Job {JobName} (ID: {JobId}) failed after {Duration:F2}s", jobName, jobId, duration);
            throw;
        }
        finally
        {
            // Clear context
            ContextManager.ClearContext();
        }
    }

    /// <summary>
    /// Implement your job logic here
    /// </summary>
    protected abstract Task ExecuteJobAsync(IJobExecutionContext context);

    /// <summary>
    /// Initialize context if tenant/user IDs are provided in job data
    /// </summary>
    private async Task InitializeContextAsync(IJobExecutionContext context)
    {
        var data = context.MergedJobDataMap;

        var tenantId = GetIntValue(data, TenantIdKey);
        var applicationUserId = GetIntValue(data, ApplicationUserIdKey);

        if (tenantId > 0 && applicationUserId > 0)
        {
            // Both tenant and user provided
            await ContextManager.InitializeContextAsync(tenantId, applicationUserId);
            Logger.LogDebug("Initialized context with tenant {TenantId} and user {ApplicationUserId}", tenantId, applicationUserId);
        }
        else if (tenantId > 0)
        {
            // Only tenant provided - use default admin
            await ContextManager.InitializeContextWithDefaultAdminAsync(tenantId);
            Logger.LogDebug("Initialized context with tenant {TenantId} using default admin", tenantId);
        }
        else if (applicationUserId > 0)
        {
            // Only user provided - use default tenant
            await ContextManager.InitializeContextWithDefaultTenantAsync(applicationUserId);
            Logger.LogDebug("Initialized context with user {ApplicationUserId} using default tenant", applicationUserId);
        }
        else
        {
            // No context initialization - system job
            Logger.LogDebug("No context initialization required - running as system job");
        }
    }

    /// <summary>
    /// Helper to get int value from JobDataMap
    /// </summary>
    private static int GetIntValue(JobDataMap data, string key)
    {
        if (data.TryGetValue(key, out var value))
        {
            if (value is int intValue) return intValue;
            if (int.TryParse(value?.ToString(), out var parsedValue)) return parsedValue;
        }
        return 0;
    }

    /// <summary>
    /// Get current tenant ID from context
    /// </summary>
    protected int? GetCurrentTenantId() => ContextManager.GetCurrentTenantId();

    /// <summary>
    /// Get current application user ID from context
    /// </summary>
    protected int? GetCurrentApplicationUserId() => ContextManager.GetCurrentApplicationUserId();

    /// <summary>
    /// Get correlation ID from job data or generate a new one
    /// </summary>
    private string GetCorrelationId(IJobExecutionContext context)
    {
        var data = context.MergedJobDataMap;
        if (data.TryGetValue(CorrelationIdKey, out var correlationId) && !string.IsNullOrEmpty(correlationId?.ToString()))
        {
            return correlationId.ToString()!;
        }

        // Generate new correlation ID if not provided
        return Guid.NewGuid().ToString("N")[..16]; // Short correlation ID
    }

    /// <summary>
    /// Get current correlation ID from job context (useful for derived classes)
    /// </summary>
    protected string GetCurrentCorrelationId(IJobExecutionContext context) => GetCorrelationId(context);

    /// <summary>
    /// Create a comprehensive LogContext scope with all job-related properties
    /// </summary>
    private IDisposable CreateLogContextScope(IJobExecutionContext context, string correlationId, string jobName, string jobId)
    {
        var disposables = new List<IDisposable>
        {
            LogContext.PushProperty("CorrelationId", correlationId),
            LogContext.PushProperty("JobName", jobName),
            LogContext.PushProperty("JobId", jobId),
            LogContext.PushProperty("FireTime", context.FireTimeUtc),
            LogContext.PushProperty("NextFireTime", context.NextFireTimeUtc)
        };

        // Add job data properties to log context
        var jobData = context.MergedJobDataMap;
        foreach (var kvp in jobData)
        {
            if (kvp.Key != CorrelationIdKey && kvp.Value != null)
            {
                disposables.Add(LogContext.PushProperty($"Job.{kvp.Key}", kvp.Value));
            }
        }

        return new CompositeDisposable(disposables);
    }

    /// <summary>
    /// Add job data properties to distributed tracing activity
    /// </summary>
    private static void AddJobDataToActivity(Activity? activity, IJobExecutionContext context)
    {
        if (activity == null) return;

        var jobData = context.MergedJobDataMap;
        foreach (var kvp in jobData)
        {
            if (kvp.Key != CorrelationIdKey && kvp.Value != null)
            {
                activity.SetTag($"job.data.{kvp.Key.ToLowerInvariant()}", kvp.Value.ToString());
            }
        }

        // Add timing information
        activity.SetTag("job.fire_time", context.FireTimeUtc.ToString("O"));
        if (context.NextFireTimeUtc.HasValue)
        {
            activity.SetTag("job.next_fire_time", context.NextFireTimeUtc.Value.ToString("O"));
        }
    }

    /// <summary>
    /// Helper class to dispose multiple IDisposable objects
    /// </summary>
    private class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables;

        public CompositeDisposable(List<IDisposable> disposables)
        {
            _disposables = disposables;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
        }
    }
}