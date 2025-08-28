using Quartz;

namespace ConnectFlow.Infrastructure.Quartz.Jobs;

/// <summary>
/// A job that simulates long-running processes that might cause misfires
/// </summary>
[DisallowConcurrentExecution]
public class LongRunningJob : BaseJob
{
    public LongRunningJob(ILogger<LongRunningJob> logger, IContextManager contextManager)
        : base(logger, contextManager)
    {
    }

    protected override async Task ExecuteJobAsync(IJobExecutionContext context)
    {
        Logger.LogInformation("LongRunningJob started at: {Time}", DateTimeOffset.Now);

        try
        {
            // Randomly decide how long this job will take
            // Make it very likely to take longer than the trigger interval (5s), causing misfires
            var duration = new Random().Next(4, 15);

            Logger.LogInformation("LongRunningJob will execute for {Duration} seconds", duration);

            // Simulate long-running work
            await Task.Delay(TimeSpan.FromSeconds(duration), context.CancellationToken);

            Logger.LogInformation("LongRunningJob completed successfully");
        }
        catch (TaskCanceledException)
        {
            Logger.LogWarning("LongRunningJob was interrupted");
            throw; // Let Quartz know the job was interrupted
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "LongRunningJob failed with error");
            throw; // Rethrow for Quartz to handle the error
        }
    }
}