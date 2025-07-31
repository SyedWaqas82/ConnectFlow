using Quartz;

namespace ConnectFlow.Infrastructure.Common.Jobs;

/// <summary>
/// A job that simulates long-running processes that might cause misfires
/// </summary>
[DisallowConcurrentExecution]
public class LongRunningJob : IJob
{
    private readonly ILogger<LongRunningJob> _logger;

    public LongRunningJob(ILogger<LongRunningJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("LongRunningJob started at: {Time}", DateTimeOffset.Now);

        try
        {
            // Randomly decide how long this job will take
            // Make it very likely to take longer than the trigger interval (5s), causing misfires
            var duration = new Random().Next(4, 15);

            _logger.LogInformation("LongRunningJob will execute for {Duration} seconds", duration);

            // Simulate long-running work
            await Task.Delay(TimeSpan.FromSeconds(duration), context.CancellationToken);

            _logger.LogInformation("LongRunningJob completed successfully");
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("LongRunningJob was interrupted");
            throw; // Let Quartz know the job was interrupted
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LongRunningJob failed with error");
            throw; // Rethrow for Quartz to handle the error
        }
    }
}