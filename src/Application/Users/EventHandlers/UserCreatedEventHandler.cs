using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Events.Mediator.Users;

namespace ConnectFlow.Application.Users.EventHandlers;

public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<UserCreatedEventHandler> _logger;

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
            IsHtml = true,
            TemplateId = EmailTemplates.Welcome,
            TemplateData = new Dictionary<string, object>
            {
                { "userPublicId", notification.PublicUserId.GetValueOrDefault() },
                { "name", $"{notification.FirstName} {notification.LastName}" },
                { "firstName", notification.FirstName },
                { "lastName", notification.LastName },
                { "email", notification.Email },
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