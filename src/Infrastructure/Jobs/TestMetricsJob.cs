using Microsoft.Extensions.Logging;
using Quartz;

namespace ConnectFlow.Infrastructure.Jobs;

/// <summary>
/// A simple test job that runs every minute to generate metrics
/// </summary>
[DisallowConcurrentExecution]
public class TestMetricsJob : IJob
{
    private readonly ILogger<TestMetricsJob> _logger;

    public TestMetricsJob(ILogger<TestMetricsJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("TestMetricsJob executed at: {Time}", DateTimeOffset.Now);

        try
        {
            // Get job data
            var data = context.MergedJobDataMap;
            var jobName = context.JobDetail.Key.Name;
            var fireTime = context.FireTimeUtc;
            var nextFireTime = context.NextFireTimeUtc;

            _logger.LogInformation(
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

            _logger.LogInformation("TestMetricsJob completed successfully in {Duration}s", duration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TestMetricsJob failed with error");
            throw; // Rethrow to let Quartz handle it as a failed job
        }
    }
}
