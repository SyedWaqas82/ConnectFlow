using ConnectFlow.Domain.Events.UserEmailEvents;
using ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ.Configurations;
using Microsoft.Extensions.Options;

namespace ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ.Consumers;

public class EmailConsumer : RabbitMQConsumerService<EmailSendRequestedEvent>
{
    public EmailConsumer(IRabbitMQConnectionManager connectionManager, IOptions<RabbitMQSettings> settings, IServiceProvider serviceProvider, ILogger<EmailConsumer> logger) : base(connectionManager, settings, serviceProvider, logger, MessagingConfiguration.Queues.Email)
    {
    }

    protected override string GetRetryQueueName()
    {
        return MessagingConfiguration.Queues.EmailRetry;
    }

    protected override string GetRetryRoutingKey()
    {
        return MessagingConfiguration.RoutingKeys.EmailRetry;
    }
}