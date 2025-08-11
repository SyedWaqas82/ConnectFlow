using ConnectFlow.Domain.Common;

namespace ConnectFlow.Application.Common.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : BaseMessageEvent;
    Task PublishBatchAsync<T>(IEnumerable<T> messages, string routingKey, CancellationToken cancellationToken = default) where T : BaseMessageEvent;
}