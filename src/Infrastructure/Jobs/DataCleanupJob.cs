using Quartz;

namespace ConnectFlow.Infrastructure.Jobs;

/// <summary>
/// Job that handles cleanup of expired data from the system
/// </summary>
[DisallowConcurrentExecution]
public class DataCleanupJob : IJob
{
    private readonly ILogger<DataCleanupJob> _logger;
    private static readonly ActivitySource ActivitySource = new("ConnectFlow.Infrastructure");

    public DataCleanupJob(ILogger<DataCleanupJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobId = context.FireInstanceId;
        var correlationId = $"job_{jobId}";

        // Create activity for tracing this background job
        using var activity = ActivitySource.StartActivity("DataCleanupJob", ActivityKind.Internal);
        activity?.SetTag("job.id", jobId);
        activity?.SetTag("job.name", context.JobDetail.Key.Name);
        activity?.SetTag("correlation.id", correlationId);

        // Use correlation ID in logs
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("JobId", jobId))
        using (LogContext.PushProperty("JobName", context.JobDetail.Key.Name))
        {
            _logger.LogInformation("Starting data cleanup job at {StartTime}", DateTimeOffset.Now);

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

                _logger.LogInformation("Data cleanup job completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during data cleanup job");
                throw; // Quartz will handle the exception and retry based on configuration
            }
        }
    }
}
