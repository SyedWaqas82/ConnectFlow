using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Events.Messaging;
using ConnectFlow.Infrastructure.Common.Models;
using Microsoft.Extensions.Options;

namespace ConnectFlow.Infrastructure.Services.Messaging.RabbitMQ.Consumers;

public class EmailConsumer : RabbitMQConsumerService<EmailSendMessageEvent>
{
    public EmailConsumer(IRabbitMQConnectionManager connectionManager, IOptions<RabbitMQSettings> settings, IServiceProvider serviceProvider, ILogger<EmailConsumer> logger) : base(connectionManager, settings, serviceProvider, logger, MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Default, MessagingConfiguration.QueueDomain.Email), MessagingConfiguration.GetQueueByTypeAndDomain(MessagingConfiguration.QueueType.Retry, MessagingConfiguration.QueueDomain.Email))
    {

    }
}