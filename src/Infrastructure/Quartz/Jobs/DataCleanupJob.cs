using Quartz;

namespace ConnectFlow.Infrastructure.Quartz.Jobs;

/// <summary>
/// Job that handles cleanup of expired data from the system
/// </summary>
[DisallowConcurrentExecution]
public class DataCleanupJob : BaseJob
{
    public DataCleanupJob(ILogger<DataCleanupJob> logger, IContextManager contextManager)
        : base(logger, contextManager)
    {
    }

    protected override async Task ExecuteJobAsync(IJobExecutionContext context)
    {
        Logger.LogInformation("Starting data cleanup job at {StartTime}", DateTimeOffset.Now);

        try
        {
            // Here you would implement your cleanup logic
            // For example:
            // - Delete expired temporary files
            // - Remove old audit logs
            // - Clean up expired tokens
            // - Archive completed workflows

            // Simulate work
            await Task.Delay(TimeSpan.FromSeconds(2), context.CancellationToken);

            Logger.LogInformation("Data cleanup job completed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during data cleanup job");
            throw; // Quartz will handle the exception and retry based on configuration
        }
    }
}