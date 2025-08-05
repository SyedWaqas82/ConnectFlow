namespace ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ;

public interface IRabbitMQSetupService
{
    Task SetupTopologyAsync(CancellationToken cancellationToken = default);
    Task EnsureExchangesAsync(CancellationToken cancellationToken = default);
    Task EnsureQueuesAsync(CancellationToken cancellationToken = default);
}