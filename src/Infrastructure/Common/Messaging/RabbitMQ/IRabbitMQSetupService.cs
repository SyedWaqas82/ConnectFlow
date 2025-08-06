namespace ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ;

public interface IRabbitMQSetupService
{
    Task SetupTopologyAsync(CancellationToken cancellationToken = default);
}