using System.Diagnostics.Metrics;
using Microsoft.Extensions.Options;
using ConnectFlow.Application.Common.Models;
using Quartz;
using Quartz.Impl.Matchers;
using System.Collections.Concurrent;

namespace ConnectFlow.Infrastructure.Common.Providers;

/// <summary>
/// Provider for Quartz job metrics that integrates with OpenTelemetry
/// </summary>
public class QuartzMetricsProvider
{
    private readonly ILogger<QuartzMetricsProvider> _logger;
    private readonly ISchedulerFactory _schedulerFactory;

    // Meter for exposing metrics to OpenTelemetry
    internal readonly Meter _meter;

    // Metrics instruments
    private readonly Counter<int> _jobsExecutedCounter;
    private readonly Counter<int> _jobMisfireCounter;
    private readonly Counter<int> _jobErrorCounter;
    private readonly Histogram<double> _jobExecutionTimeHistogram;

    // Dictionary to track job executions
    private readonly ConcurrentDictionary<string, Stopwatch> _jobExecutionTimers = new();

    /// <summary>
    /// Creates a new instance of the Quartz metrics provider
    /// </summary>
    public QuartzMetricsProvider(ILogger<QuartzMetricsProvider> logger, ISchedulerFactory schedulerFactory, IOptions<QuartzSettings> settings)
    {
        _logger = logger;
        _schedulerFactory = schedulerFactory;

        _logger.LogInformation("Initializing Quartz metrics provider");

        // Create meter for metrics - matches the meter name in OpenTelemetry configuration
        _meter = new Meter("ConnectFlow.Metrics", "1.0.0");

        // Create metrics instruments - match exact names from dashboard
        _jobsExecutedCounter = _meter.CreateCounter<int>("quartz_jobs_executed_count_total", description:
            "Number of jobs executed by the Quartz scheduler");

        _jobMisfireCounter = _meter.CreateCounter<int>("quartz_jobs_misfire_count_total", description:
            "Number of job misfires detected by the Quartz scheduler");

        _jobErrorCounter = _meter.CreateCounter<int>("quartz_jobs_error_count_total", description:
            "Number of job execution errors by job name");

        _jobExecutionTimeHistogram = _meter.CreateHistogram<double>("quartz_job_execution_time_seconds", description:
            "Execution time of Quartz jobs");

        // Additional counters for more dashboard metrics
        _meter.CreateCounter<int>("quartz_jobs_scheduled_total", description:
            "Number of jobs scheduled");

        _meter.CreateCounter<int>("quartz_jobs_completed_total", description:
            "Number of jobs completed successfully");

        _meter.CreateCounter<int>("quartz_triggers_fired_total", description:
            "Number of triggers fired");

        // Create observable metrics - match exactly what dashboard expects
        _meter.CreateObservableGauge("quartz_scheduler_running",
            () => GetSchedulerMetric(s => s.IsStarted && !s.IsShutdown ? 1 : 0),
            description: "Indicates if the Quartz scheduler is running");

        _meter.CreateObservableGauge("quartz_scheduler_jobs_total",
            () => GetSchedulerMetric(async s => (await s.GetJobKeys(GroupMatcher<JobKey>.AnyGroup())).Count),
            description: "Total number of jobs in the Quartz scheduler");

        _meter.CreateObservableGauge("quartz_scheduler_jobs_executing",
            () => GetSchedulerMetric(async s => (await s.GetCurrentlyExecutingJobs()).Count),
            description: "Number of currently executing jobs");

        _meter.CreateObservableGauge("quartz_scheduler_queue_depth",
            GetQueueDepth,
            description: "Number of jobs waiting in the queue");

        _meter.CreateObservableGauge("quartz_scheduler_time_to_next_job_seconds",
            GetTimeToNextJob,
            description: "Time to the next scheduled job execution");

        // Additional metrics that might be needed for all dashboard panels
        _meter.CreateObservableGauge("quartz_scheduler_triggers_total",
            () => GetSchedulerMetric(async s => (await s.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup())).Count),
            description: "Total number of triggers in the Quartz scheduler");

        _meter.CreateObservableGauge("quartz_scheduler_threads_total",
            () => 10, // This should match the MaxConcurrency setting in QuartzConfiguration
            description: "Total number of worker threads in the Quartz scheduler");

        _meter.CreateObservableGauge("quartz_scheduler_threads_active",
            () => GetSchedulerMetric(async s => (await s.GetCurrentlyExecutingJobs()).Count),
            description: "Number of active worker threads in the Quartz scheduler");

        // Add some additional metrics that might be useful
        _meter.CreateObservableGauge("quartz_scheduler_info",
            () =>
            {
                return new Measurement<double>(
                    1.0,
                    new KeyValuePair<string, object?>("scheduler_name", settings.Value.SchedulerName),
                    new KeyValuePair<string, object?>("instance_id", settings.Value.InstanceId),
                    new KeyValuePair<string, object?>("version", "1.0.0")
                );
            },
            "info",
            "Quartz scheduler information with scheduler name, instance id, and version as tags");

        _logger.LogInformation("Quartz metrics registered with ConnectFlow.Metrics meter");

        // Start the initialization process asynchronously but safely
        _ = InitializeAsync();
    }

    /// <summary>
    /// Initializes the job and trigger listeners
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InitializeAsync()
    {
        try
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            if (scheduler == null)
            {
                _logger.LogWarning("Could not get scheduler to initialize metrics");
                return;
            }

            // Create and register listeners
            var listener = new MetricsListener(this);

            scheduler.ListenerManager.AddJobListener(listener, GroupMatcher<JobKey>.AnyGroup());
            scheduler.ListenerManager.AddTriggerListener(listener, GroupMatcher<TriggerKey>.AnyGroup());

            // Refresh metrics immediately to get initial values
            RefreshMetrics();

            _logger.LogInformation("Quartz metrics provider initialized with job and trigger listeners");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Quartz metrics listeners");
        }
    }

    /// <summary>
    /// Forces an immediate refresh of all metrics
    /// </summary>
    public void RefreshMetrics()
    {
        try
        {
            // Force evaluation of metrics to ensure they're registered
            var queueDepth = GetQueueDepth();
            var timeToNext = GetTimeToNextJob();

            // Force evaluation of additional metrics
            var scheduler = _schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            if (scheduler != null && !scheduler.IsShutdown)
            {
                var jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).GetAwaiter().GetResult();
                var triggerKeys = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup()).GetAwaiter().GetResult();
                var executingJobs = scheduler.GetCurrentlyExecutingJobs().GetAwaiter().GetResult();

                _logger.LogDebug("Metrics refreshed - Queue depth: {QueueDepth}, Time to next job: {TimeToNext}s",
                    queueDepth, timeToNext);

                _logger.LogDebug("Additional metrics - Jobs: {JobCount}, Triggers: {TriggerCount}, Executing: {ExecutingCount}",
                    jobKeys.Count, triggerKeys.Count, executingJobs.Count);

                // Add sample data points for each counter to ensure they appear in Grafana
                // Use the exact metric names that the dashboard expects
                _meter.CreateCounter<int>("quartz_jobs_scheduled_total").Add(0);
                _meter.CreateCounter<int>("quartz_jobs_completed_total").Add(0);
                _meter.CreateCounter<int>("quartz_triggers_fired_total").Add(0);
                _meter.CreateCounter<int>("quartz_jobs_executed_count_total").Add(0,
                    new KeyValuePair<string, object?>("success", "true"),
                    new KeyValuePair<string, object?>("job_name", "Sample"));
                _meter.CreateCounter<int>("quartz_jobs_executed_count_total").Add(0,
                    new KeyValuePair<string, object?>("success", "false"),
                    new KeyValuePair<string, object?>("job_name", "Sample"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh metrics");
        }
    }

    /// <summary>
    /// Safely executes a function against the scheduler to retrieve a metric value
    /// </summary>
    private T GetSchedulerMetric<T>(Func<IScheduler, T> metricFunc, T defaultValue = default!) where T : notnull
    {
        try
        {
            var scheduler = _schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            if (scheduler != null && !scheduler.IsShutdown)
            {
                return metricFunc(scheduler);
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error retrieving scheduler metric");
        }
        return defaultValue;
    }

    /// <summary>
    /// Safely executes an async function against the scheduler to retrieve a metric value
    /// </summary>
    private int GetSchedulerMetric<T>(Func<IScheduler, Task<T>> asyncMetricFunc, int defaultValue = 0)
    {
        try
        {
            var scheduler = _schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            if (scheduler != null && !scheduler.IsShutdown)
            {
                return asyncMetricFunc(scheduler).GetAwaiter().GetResult() is int value ? value : defaultValue;
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error retrieving scheduler metric");
        }
        return defaultValue;
    }

    /// <summary>
    /// Gets the number of jobs in the queue (ready to fire but not yet fired)
    /// </summary>
    private int GetQueueDepth()
    {
        try
        {
            var scheduler = _schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            if (scheduler == null || scheduler.IsShutdown)
                return 0;

            var triggers = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup()).GetAwaiter().GetResult();
            if (triggers.Count == 0)
                return 0;

            int queuedCount = 0;
            var now = DateTimeOffset.UtcNow;

            foreach (var triggerKey in triggers)
            {
                var trigger = scheduler.GetTrigger(triggerKey).GetAwaiter().GetResult();
                if (trigger == null) continue;

                var nextFireTime = trigger.GetNextFireTimeUtc();
                if (!nextFireTime.HasValue) continue;

                // If the next fire time is in the past or very near future (within 100ms),
                // consider it queued
                if (nextFireTime.Value <= now.AddMilliseconds(100))
                {
                    queuedCount++;
                }
            }

            return queuedCount;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error calculating queue depth");
            return 0;
        }
    }

    /// <summary>
    /// Gets the time in seconds to the next scheduled job
    /// </summary>
    private double GetTimeToNextJob()
    {
        try
        {
            var scheduler = _schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            if (scheduler == null || scheduler.IsShutdown)
                return -1;

            var triggers = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup()).GetAwaiter().GetResult();
            if (triggers.Count == 0)
                return -1;

            DateTimeOffset? nextFireTime = null;

            foreach (var triggerKey in triggers)
            {
                var trigger = scheduler.GetTrigger(triggerKey).GetAwaiter().GetResult();
                if (trigger == null) continue;

                var triggerNextFireTime = trigger.GetNextFireTimeUtc();
                if (!triggerNextFireTime.HasValue) continue;

                if (!nextFireTime.HasValue || triggerNextFireTime.Value < nextFireTime.Value)
                    nextFireTime = triggerNextFireTime.Value;
            }

            if (!nextFireTime.HasValue)
                return -1;

            var timeToNext = nextFireTime.Value.ToLocalTime() - DateTimeOffset.Now;
            return Math.Max(0, timeToNext.TotalSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error calculating time to next job");
            return -1;
        }
    }

    /// <summary>
    /// Records job execution metrics
    /// </summary>
    public void RecordJobExecution(string jobName, bool success, double executionTime)
    {
        _jobsExecutedCounter.Add(1, new KeyValuePair<string, object?>("job_name", jobName),
                                   new KeyValuePair<string, object?>("success", success));

        _jobExecutionTimeHistogram.Record(executionTime,
            new KeyValuePair<string, object?>("job_name", jobName));

        if (!success)
        {
            _jobErrorCounter.Add(1, new KeyValuePair<string, object?>("job_name", jobName));
        }

        // Log detailed metrics info to help with debugging
        _logger.LogDebug(
            "Recorded job execution: {JobName}, Success: {Success}, Time: {ExecutionTime}s",
            jobName, success, executionTime);

        // Add extra entries just for the Grafana dashboard - these need to match expected format
        if (success)
        {
            _meter.CreateCounter<int>("quartz_jobs_executed_count_total").Add(1,
                new KeyValuePair<string, object?>("job_name", jobName),
                new KeyValuePair<string, object?>("success", "true"));
        }
        else
        {
            _meter.CreateCounter<int>("quartz_jobs_executed_count_total").Add(1,
                new KeyValuePair<string, object?>("job_name", jobName),
                new KeyValuePair<string, object?>("success", "false"));
        }
    }

    /// <summary>
    /// Records a job misfire event
    /// </summary>
    public void RecordJobMisfire(string jobName)
    {
        _jobMisfireCounter.Add(1, new KeyValuePair<string, object?>("job_name", jobName));

        // Log for debugging
        _logger.LogDebug("Recorded job misfire: {JobName}", jobName);
    }

    /// <summary>
    /// Combined job and trigger listener for metrics
    /// </summary>
    internal class MetricsListener : IJobListener, ITriggerListener
    {
        private readonly QuartzMetricsProvider _metricsProvider;
        private readonly ConcurrentDictionary<string, Stopwatch> _jobTimers = new();

        public MetricsListener(QuartzMetricsProvider metricsProvider)
        {
            _metricsProvider = metricsProvider;
        }

        public string Name => "QuartzMetricsListener";

        // IJobListener methods
        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var jobName = context.JobDetail.Key.ToString();

            // Start timing
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _jobTimers[jobName] = stopwatch;

            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var jobName = context.JobDetail.Key.ToString();

            // Remove from timing dictionary
            _jobTimers.TryRemove(jobName, out _);

            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException,
            CancellationToken cancellationToken = default)
        {
            var jobName = context.JobDetail.Key.ToString();

            if (_jobTimers.TryRemove(jobName, out var stopwatch))
            {
                stopwatch.Stop();
                var executionTime = stopwatch.Elapsed.TotalSeconds;

                // Record the execution
                _metricsProvider.RecordJobExecution(jobName, jobException == null, executionTime);

                // Increment the completed jobs counter if successful
                if (jobException == null)
                {
                    _metricsProvider._meter.CreateCounter<int>("quartz_jobs_completed_total").Add(1,
                        new KeyValuePair<string, object?>("job_name", jobName));
                }
            }

            return Task.CompletedTask;
        }

        // ITriggerListener methods
        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            // Increment the triggers fired counter
            var meter = _metricsProvider._meter;
            meter.CreateCounter<int>("quartz_triggers_fired_total").Add(1,
                new KeyValuePair<string, object?>("trigger_name", trigger.Key.Name),
                new KeyValuePair<string, object?>("job_name", trigger.JobKey.ToString()));

            // Increment the jobs scheduled counter
            meter.CreateCounter<int>("quartz_jobs_scheduled_total").Add(1,
                new KeyValuePair<string, object?>("job_name", trigger.JobKey.ToString()));

            return Task.CompletedTask;
        }

        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false); // Don't veto any jobs
        }

        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            var jobKey = trigger.JobKey;
            _metricsProvider.RecordJobMisfire(jobKey.ToString());

            return Task.CompletedTask;
        }

        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context,
            SchedulerInstruction triggerInstruction, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
