using Microsoft.Extensions.Logging;
using Quartz;
using Serilog.Context;
using System.Diagnostics;

namespace ConnectFlow.Infrastructure.Jobs;

/// <summary>
/// Job that generates periodic reports for the system
/// </summary>
[DisallowConcurrentExecution]
public class ReportGenerationJob : IJob
{
    private readonly ILogger<ReportGenerationJob> _logger;
    private static readonly ActivitySource ActivitySource = new("ConnectFlow.Infrastructure");

    public ReportGenerationJob(ILogger<ReportGenerationJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobId = context.FireInstanceId;
        var correlationId = $"job_{jobId}";
        var reportType = context.MergedJobDataMap.GetString("ReportType") ?? "System";

        // Create activity for tracing
        using var activity = ActivitySource.StartActivity("ReportGenerationJob", ActivityKind.Internal);
        activity?.SetTag("job.id", jobId);
        activity?.SetTag("job.name", context.JobDetail.Key.Name);
        activity?.SetTag("correlation.id", correlationId);
        activity?.SetTag("report.type", reportType);

        // Use correlation ID and job data in logs
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("JobId", jobId))
        using (LogContext.PushProperty("ReportType", reportType))
        {
            _logger.LogInformation("Starting {ReportType} report generation at {StartTime}", reportType, DateTimeOffset.Now);

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

                _logger.LogInformation("{ReportType} report generated successfully", reportType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during {ReportType} report generation", reportType);
                throw; // Quartz will handle the exception
            }
        }
    }
}
