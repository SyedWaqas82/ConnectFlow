using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Events.UserEmailEvents;
using Microsoft.Extensions.Logging;

namespace ConnectFlow.Application.Users.EventHandlers.Messaging;

public class EmailSendRequestedHandler : IMessageHandler<EmailSendRequestedEvent>
{
    private readonly ILogger<EmailSendRequestedHandler> _logger;

    public EmailSendRequestedHandler(ILogger<EmailSendRequestedHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(EmailSendRequestedEvent message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing email send request for {To} with subject {Subject} (Tenant: {TenantId} {UserId})", message.To, message.Subject, message.TenantId, message.UserId);

            // Successfully sent email
            //await _messagePublisher.PublishAsync(sentEvent, "event.email.sent", message.TenantId, cancellationToken);
            message.Acknowledge();
            //_logger.LogInformation("Email sent successfully to {To} (MessageId: {MessageId})", message.To, message.MessageId);

            // If email sending fails, publish a failed event
            //await _messagePublisher.PublishAsync(failedEvent, "event.email.failed", context.TenantId, cancellationToken);
            // if (message.RetryCount < 3)
            // {
            //     message.Reject(requeue: false); // Will be sent to retry queue
            //     _logger.LogWarning("Email sending failed, will retry. To: {To}, Error: {Error}", message.To, message.MessageId);
            // }
            // else
            // {
            //     message.Acknowledge(); // Don't retry anymore
            //     _logger.LogError("Email sending failed permanently. To: {To}, Error: {Error}", message.To, message.MessageId);
            // }

            await Task.CompletedTask; // Simulate async operation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing email send request for {To}", message.To);
            message.Reject(requeue: false);
        }
    }
}