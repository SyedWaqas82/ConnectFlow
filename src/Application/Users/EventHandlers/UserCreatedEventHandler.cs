using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Events.Mediator.Users;
using ConnectFlow.Domain.Events.Messaging;
using Microsoft.Extensions.Logging;

namespace ConnectFlow.Application.Users.EventHandlers;

public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;
    private readonly IMessagePublisher _messagePublisher;

    public UserCreatedEventHandler(IMessagePublisher messagePublisher, ILogger<UserCreatedEventHandler> logger)
    {
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ConnectFlow Domain Event: {DomainEvent}", notification.GetType().Name);

        var emailEvent = new EmailSendMessageEvent()
        {
            CorrelationId = notification.CorrelationId,
            TenantId = notification.TenantId,
            ApplicationUserId = notification.ApplicationUserId,
            PublicUserId = notification.PublicUserId,
            To = notification.Email,
            Subject = "Welcome to ConnectFlow!",
            Cc = notification.Cc,
            Bcc = notification.Bcc,
            IsHtml = true,
            TemplateId = EmailTemplates.Welcome,
            TemplateData = new Dictionary<string, object>
            {
                { "firstName", notification.FirstName },
                { "lastName", notification.LastName },
                { "email", notification.Email },
                { "name", $"{notification.FirstName} {notification.LastName}" },
                { "jobTitle", notification.JobTitle ?? string.Empty },
                { "phoneNumber", notification.PhoneNumber ?? string.Empty },
                { "mobile", notification.Mobile ?? string.Empty },
                { "timeZone", notification.TimeZone ?? string.Empty },
                { "locale", notification.Locale ?? string.Empty },
                { "emailConfirmed", notification.EmailConfirmed },
                { "confirmationToken", notification.ConfirmationToken },
                { "applicationUserId", notification.ApplicationUserId.GetValueOrDefault() },
                { "publicUserId", notification.PublicUserId.GetValueOrDefault() },
                { "tenantId", notification.TenantId.GetValueOrDefault() },
                { "correlationId", notification.CorrelationId.GetValueOrDefault() }
            },
        };

        var queue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Email);

        await _messagePublisher.PublishAsync(emailEvent, queue.RoutingKey, cancellationToken);
    }
}