using ConnectFlow.Domain.Common;

namespace ConnectFlow.Application.Common.Messaging;

public interface IMessageHandler<in T> where T : MessageBaseEvent
{
    Task HandleAsync(T message, CancellationToken cancellationToken = default);
}