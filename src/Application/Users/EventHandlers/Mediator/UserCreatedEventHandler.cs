using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Events.Mediator.Users;
using ConnectFlow.Domain.Events.UserEmailEvents;
using Microsoft.Extensions.Logging;

namespace ConnectFlow.Application.Users.EventHandlers.Mediator;

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

        //await _messagePublisher.PublishAsync(notification, MessagingConfiguration.RoutingKeys.Email, cancellationToken);

        var emailEvent = new EmailSendRequestedEvent()
        {
            UserId = notification.UserId,
        };

        await _messagePublisher.PublishAsync(emailEvent, "event.email", cancellationToken);

    }
}