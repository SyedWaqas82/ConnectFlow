using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Events.UserEmailEvents;
using ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ.Configurations;
using Microsoft.Extensions.Options;

namespace ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ.Consumers;

public class EmailConsumer : RabbitMQConsumerService<EmailSendRequestedEvent>
{
    public EmailConsumer(IRabbitMQConnectionManager connectionManager, IOptions<RabbitMQSettings> settings, IServiceProvider serviceProvider, ILogger<EmailConsumer> logger) : base(connectionManager, settings, serviceProvider, logger, MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Email), MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Retry, MessagingConfiguration.QueueDomain.Email))
    {

    }
}