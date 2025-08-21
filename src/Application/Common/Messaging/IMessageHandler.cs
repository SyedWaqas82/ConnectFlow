namespace ConnectFlow.Application.Common.Messaging;

public interface IMessageHandler<in T> where T : BaseMessageEvent
{
    Task HandleAsync(T message, CancellationToken cancellationToken = default);
}