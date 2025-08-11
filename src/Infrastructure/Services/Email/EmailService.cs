using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly IEmailSender _sender;
    private readonly IEmailTemplateRenderer _renderer;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IEmailSender sender, IEmailTemplateRenderer renderer, ILogger<EmailService> logger)
    {
        _sender = sender;
        _renderer = renderer;
        _logger = logger;
    }

    public async Task<Result<string>> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(message.TemplateId))
        {
            var model = message.TemplateData ?? new Dictionary<string, object>();
            message.Body = await _renderer.RenderAsync(message.TemplateId, model, cancellationToken);
            message.IsHtml = true;
        }

        var result = await _sender.SendAsync(message, cancellationToken);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Email send failed for {To}: {Error}", message.To, result.Errors.FirstOrDefault());
        }
        return result;
    }
}