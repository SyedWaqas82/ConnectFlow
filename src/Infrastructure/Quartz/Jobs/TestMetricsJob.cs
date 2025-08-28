using Quartz;

namespace ConnectFlow.Infrastructure.Quartz.Jobs;

/// <summary>
/// A simple test job that runs every minute to generate metrics
/// </summary>
[DisallowConcurrentExecution]
public class TestMetricsJob : BaseJob
{
    public TestMetricsJob(ILogger<TestMetricsJob> logger, IContextManager contextManager) : base(logger, contextManager)
    {
    }

    protected override async Task ExecuteJobAsync(IJobExecutionContext context)
    {
        Logger.LogInformation("TestMetricsJob executed at: {Time}", DateTimeOffset.UtcNow);

        try
        {
            // Get job data
            var data = context.MergedJobDataMap;
            var jobName = context.JobDetail.Key.Name;
            var fireTime = context.FireTimeUtc;
            var nextFireTime = context.NextFireTimeUtc;

            Logger.LogInformation(
                "Job {JobKey} executing (Fire time: {FireTime}, Next fire time: {NextFireTime})",
                jobName, fireTime, nextFireTime);

            // Randomly simulate errors to generate error metrics (roughly 20% of the time)
            if (new Random().Next(5) == 0)
            {
                throw new Exception("Simulated error for metrics testing");
            }

            // Simulate varying work durations
            var duration = new Random().Next(1, 4);
            await Task.Delay(TimeSpan.FromSeconds(duration), context.CancellationToken);

            Logger.LogInformation("TestMetricsJob completed successfully in {Duration}s", duration);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "TestMetricsJob failed with error");
            throw; // Rethrow to let Quartz handle it as a failed job
        }
    }
}