using Quartz;

namespace ConnectFlow.Infrastructure.Quartz.Jobs;

/// <summary>
/// Job that generates periodic reports for the system
/// </summary>
[DisallowConcurrentExecution]
public class ReportGenerationJob : BaseJob
{
    public ReportGenerationJob(ILogger<ReportGenerationJob> logger, IContextManager contextManager)
        : base(logger, contextManager)
    {
    }

    protected override async Task ExecuteJobAsync(IJobExecutionContext context)
    {
        var reportType = context.MergedJobDataMap.GetString("ReportType") ?? "System";

        Logger.LogInformation("Starting {ReportType} report generation at {StartTime}", reportType, DateTimeOffset.UtcNow);

        try
        {
            // Here you would implement your report generation logic
            // For example:
            // - Query database for reporting data
            // - Format reports based on templates
            // - Export to PDF/Excel
            // - Send via email or store in document management system

            // Simulate work
            await Task.Delay(TimeSpan.FromSeconds(3), context.CancellationToken);

            Logger.LogInformation("{ReportType} report generated successfully", reportType);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during {ReportType} report generation", reportType);
            throw; // Quartz will handle the exception
        }
    }
}