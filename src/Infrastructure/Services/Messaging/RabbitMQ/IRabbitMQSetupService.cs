namespace ConnectFlow.Infrastructure.Services.Messaging.RabbitMQ;

public interface IRabbitMQSetupService
{
    Task SetupTopologyAsync(CancellationToken cancellationToken = default);
}