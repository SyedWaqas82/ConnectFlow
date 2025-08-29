using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Events.Messaging;
using ConnectFlow.Infrastructure.Common.Models;

namespace ConnectFlow.Infrastructure.Services.Messaging.RabbitMQ.Consumers;

public class EmailConsumer : RabbitMQConsumerService<EmailSendMessageEvent>
{
    private static readonly MessagingConfiguration.Queue _defaultQueue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Email);
    private static readonly MessagingConfiguration.Queue _retryQueue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Retry, MessagingConfiguration.QueueDomain.Email);

    public EmailConsumer(IRabbitMQConnectionManager connectionManager, IOptions<RabbitMQSettings> settings, IServiceProvider serviceProvider, ILogger<EmailConsumer> logger)
        : base(connectionManager, settings, serviceProvider, logger, _defaultQueue, _retryQueue)
    {

    }
}