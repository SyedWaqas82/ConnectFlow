using System.Diagnostics.Metrics;

namespace ConnectFlow.Infrastructure.Metrics;

/// <summary>
/// Provides metrics tracking for email service operations
/// </summary>
public class EmailMetrics
{
    private readonly Counter<long> _emailsSent;
    private readonly Counter<long> _emailsFailed;
    private readonly Histogram<double> _emailSendDuration;
    private readonly Histogram<double> _templateRenderDuration;
    private readonly Counter<long> _attachmentsProcessed;
    private readonly Counter<long> _templateRenderFailed;
    private readonly Counter<long> _templateRenders;

    public EmailMetrics(IMeterFactory meterFactory)
    {
        // Create a meter for email metrics
        var meter = meterFactory.Create("ConnectFlow.Email");

        // Create metrics instruments
        _emailsSent = meter.CreateCounter<long>("email_sent_total", description: "Total number of emails sent successfully");
        _emailsFailed = meter.CreateCounter<long>("email_failed_total", description: "Total number of emails that failed to send");
        _emailSendDuration = meter.CreateHistogram<double>("email_send_duration_seconds", unit: "s", description: "Time taken to send emails");
        _templateRenderDuration = meter.CreateHistogram<double>("email_template_render_duration_seconds", unit: "s", description: "Time taken to render email templates");
        _templateRenderFailed = meter.CreateCounter<long>("email_template_render_failed_total", description: "Total number of failed email template renders");
        _templateRenders = meter.CreateCounter<long>("email_template_render_total", description: "Total number of email template renders");
        _attachmentsProcessed = meter.CreateCounter<long>("email_attachments_total", description: "Total number of email attachments processed");
    }

    /// <summary>
    /// Record an email being successfully sent
    /// </summary>
    /// <param name="template">The template ID used (or "direct" if no template)</param>
    public void EmailSent(string? template = null)
    {
        _emailsSent.Add(1, new KeyValuePair<string, object?>("template", template ?? "direct"));
    }

    /// <summary>
    /// Record an email send failure
    /// </summary>
    /// <param name="errorType">The type of error that occurred</param>
    /// <param name="template">The template ID used (or "direct" if no template)</param>
    public void EmailFailed(string errorType, string? template = null)
    {
        _emailsFailed.Add(1,
            new KeyValuePair<string, object?>("error_type", errorType),
            new KeyValuePair<string, object?>("template", template ?? "direct"));
    }

    /// <summary>
    /// Record the duration of an email send operation
    /// </summary>
    /// <param name="durationSeconds">Time taken in seconds</param>
    /// <param name="template">The template ID used (or "direct" if no template)</param>
    public void RecordEmailSendDuration(double durationSeconds, string? template = null)
    {
        _emailSendDuration.Record(durationSeconds, new KeyValuePair<string, object?>("template", template ?? "direct"));
    }

    /// <summary>
    /// Record the duration of a template render operation
    /// </summary>
    /// <param name="durationSeconds">Time taken in seconds</param>
    /// <param name="templateId">The template ID</param>
    public void RecordTemplateRenderDuration(double durationSeconds, string templateId)
    {
        _templateRenderDuration.Record(durationSeconds, new KeyValuePair<string, object?>("template_id", templateId));
    }

    /// <summary>
    /// Record a successful template render
    /// </summary>
    /// <param name="templateId">The template ID</param>
    public void TemplateRendered(string templateId)
    {
        _templateRenders.Add(1, new KeyValuePair<string, object?>("template_id", templateId));
    }

    /// <summary>
    /// Record a failed template render
    /// </summary>
    /// <param name="templateId">The template ID</param>
    /// <param name="errorType">The type of error that occurred</param>
    public void TemplateRenderFailed(string templateId, string errorType)
    {
        _templateRenderFailed.Add(1,
            new KeyValuePair<string, object?>("template_id", templateId),
            new KeyValuePair<string, object?>("error_type", errorType));
    }

    /// <summary>
    /// Record attachments being processed
    /// </summary>
    /// <param name="count">Number of attachments</param>
    public void RecordAttachments(int count)
    {
        _attachmentsProcessed.Add(count);
    }
}
