using ConnectFlow.Application.Common.Models;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Common.Messaging.Handlers;

public class EmailSendMessageEventHandler : IMessageHandler<EmailSendMessageEvent>
{
    private readonly IEmailService _emailService;
    private readonly MessagingConfiguration.Queue _queue;
    private readonly ILogger<EmailSendMessageEventHandler> _logger;

    public EmailSendMessageEventHandler(IEmailService emailService, ILogger<EmailSendMessageEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;

        _queue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Email);
    }

    public async Task HandleAsync(EmailSendMessageEvent message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing email send request for {To} with subject {Subject} (Tenant: {TenantId} {UserId})", message.To, message.Subject, message.TenantId, message.ApplicationUserId);

            var email = new EmailMessage
            {
                To = message.To,
                Cc = message.Cc,
                Bcc = message.Bcc,
                Subject = message.Subject,
                Body = message.Body,
                IsHtml = message.IsHtml,
                TemplateId = message.TemplateId,
                TemplateData = message.TemplateData,
            };

            var result = await _emailService.SendAsync(email, cancellationToken);
            if (result.Succeeded)
            {
                message.Acknowledge();
                _logger.LogInformation("Email sent successfully to {To} (MessageId: {MessageId})", message.To, result.Data);
            }
            else
            {
                // Implement basic retry strategy based on RetryCount in message
                var shouldRetry = message.RetryCount < _queue.MaxRetries;
                message.Reject(requeue: shouldRetry);
                _logger.LogWarning("Email sending failed for {To}. Error: {Error}. WillRetry: {Retry}", message.To, result.Errors.FirstOrDefault(), shouldRetry);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing email send request for {To}", message.To);
            // Let the infrastructure retry
            message.Reject(requeue: message.RetryCount < _queue.MaxRetries);
        }
    }
}