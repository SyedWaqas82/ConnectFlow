using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Events.Messaging;
using ConnectFlow.Infrastructure.Common.Models;

namespace ConnectFlow.Infrastructure.Services.Messaging.RabbitMQ.Consumers;

public class SubscriptionConsumer : RabbitMQConsumerService<SubscriptionMessageEvent>
{
    private static readonly MessagingConfiguration.Queue _defaultQueue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Subscription);
    private static readonly MessagingConfiguration.Queue _retryQueue = MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Retry, MessagingConfiguration.QueueDomain.Subscription);

    public SubscriptionConsumer(IRabbitMQConnectionManager connectionManager, IOptions<RabbitMQSettings> settings, IServiceProvider serviceProvider, ILogger<SubscriptionConsumer> logger)
        : base(connectionManager, settings, serviceProvider, logger, _defaultQueue, _retryQueue)
    {
    }
}