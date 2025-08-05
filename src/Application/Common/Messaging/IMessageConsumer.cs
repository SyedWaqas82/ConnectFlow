namespace ConnectFlow.Application.Common.Messaging;

public interface IMessageConsumer
{
    Task StartConsumingAsync(CancellationToken cancellationToken = default);
    Task StopConsumingAsync();
}