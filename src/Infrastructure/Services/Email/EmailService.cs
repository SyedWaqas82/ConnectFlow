using ConnectFlow.Application.Common.Models;
using ConnectFlow.Infrastructure.Metrics;

namespace ConnectFlow.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly IEmailSender _sender;
    private readonly IEmailTemplateRenderer _renderer;
    private readonly ILogger<EmailService> _logger;
    private readonly EmailMetrics _metrics;

    public EmailService(
        IEmailSender sender,
        IEmailTemplateRenderer renderer,
        ILogger<EmailService> logger,
        EmailMetrics metrics)
    {
        _sender = sender;
        _renderer = renderer;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<Result<string>> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        string? templateId = null;

        try
        {
            // Record attachments if any
            if (message.Attachments?.Count > 0)
            {
                _metrics.RecordAttachments(message.Attachments.Count);
            }

            // Handle template rendering with metrics
            if (!string.IsNullOrWhiteSpace(message.TemplateId))
            {
                templateId = message.TemplateId;
                var renderStopwatch = Stopwatch.StartNew();
                try
                {
                    var model = message.TemplateData ?? new Dictionary<string, object>();
                    message.Body = await _renderer.RenderAsync(message.TemplateId, model, cancellationToken);
                    message.IsHtml = true;

                    renderStopwatch.Stop();
                    _metrics.RecordTemplateRenderDuration(renderStopwatch.Elapsed.TotalSeconds, templateId);
                    _metrics.TemplateRendered(templateId);
                }
                catch (Exception ex)
                {
                    renderStopwatch.Stop();
                    _metrics.TemplateRenderFailed(templateId, ex.GetType().Name);
                    throw;
                }
            }

            // Send email and track metrics
            var result = await _sender.SendAsync(message, cancellationToken);

            stopwatch.Stop();
            _metrics.RecordEmailSendDuration(stopwatch.Elapsed.TotalSeconds, templateId);

            if (result.Succeeded)
            {
                _metrics.EmailSent(templateId);
                return result;
            }
            else
            {
                _logger.LogWarning("Email send failed for {To}: {Error}", message.To, result.Errors.FirstOrDefault());
                _metrics.EmailFailed(result.Errors.FirstOrDefault() ?? "Unknown", templateId);
                return result;
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Exception sending email to {To}", message.To);
            _metrics.EmailFailed(ex.GetType().Name, templateId);
            return (Result<string>)Result<string>.Failure(new[] { ex.Message });
        }
    }
}