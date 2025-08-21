using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Events.Mediator.Users;

namespace ConnectFlow.Application.Users.EventHandlers;

public class UserPasswordResetEventHandler : INotificationHandler<UserPasswordResetEvent>
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<UserPasswordResetEventHandler> _logger;

    public UserPasswordResetEventHandler(IMessagePublisher messagePublisher, ILogger<UserPasswordResetEventHandler> logger)
    {
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task Handle(UserPasswordResetEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConnectFlow Domain Event: {DomainEvent}", notification.GetType().Name);

        var emailMessage = new EmailSendMessageEvent
        {
            CorrelationId = notification.CorrelationId,
            TenantId = notification.TenantId,
            ApplicationUserId = notification.ApplicationUserId,
            PublicUserId = notification.PublicUserId,
            To = notification.Email,
            Subject = "Password Reset Request",
            IsHtml = true,
            TemplateId = EmailTemplates.PasswordReset,
            TemplateData = new Dictionary<string, object>
            {
                { "userPublicId", notification.PublicUserId.GetValueOrDefault() },
                { "name", $"{notification.FirstName} {notification.LastName}" },
                { "firstName", notification.FirstName },
                { "lastName", notification.LastName },
                { "resetPasswordToken", notification.ResetPasswordToken }
            }
        };

        var queue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Email);

        await _messagePublisher.PublishAsync(emailMessage, queue.RoutingKey, cancellationToken);
    }
}