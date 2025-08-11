using ConnectFlow.Domain.Constants;
using RabbitMQ.Client;

namespace ConnectFlow.Infrastructure.Services.Messaging.RabbitMQ;

public class RabbitMQSetupService : IRabbitMQSetupService
{
    private readonly IRabbitMQConnectionManager _connectionManager;
    private readonly ILogger<RabbitMQSetupService> _logger;

    public RabbitMQSetupService(IRabbitMQConnectionManager connectionManager, ILogger<RabbitMQSetupService> logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task SetupTopologyAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting up RabbitMQ topology...");

        await EnsureExchangesAsync(cancellationToken);

        _logger.LogInformation("RabbitMQ topology setup completed");
    }

    private async Task EnsureExchangesAsync(CancellationToken cancellationToken = default)
    {
        using var channel = await _connectionManager.CreateChannelAsync();

        var exchangeConfig = MessagingConfiguration.GetExchange();

        // Main domain events exchange
        await channel.ExchangeDeclareAsync(
            exchange: exchangeConfig.DomainEvents,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Declared exchange {ExchangeName}", exchangeConfig.DomainEvents);

        // Dead letter exchange
        await channel.ExchangeDeclareAsync(
            exchange: exchangeConfig.DeadLetter,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Declared exchange {ExchangeName}", exchangeConfig.DeadLetter);

        // Retry exchange
        await channel.ExchangeDeclareAsync(
            exchange: exchangeConfig.Retry,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Declared exchange {ExchangeName}", exchangeConfig.Retry);

        await EnsureQueuesAsync(exchangeConfig, cancellationToken);
    }

    private async Task EnsureQueuesAsync(MessagingConfiguration.Exchange exchangeConfig, CancellationToken cancellationToken = default)
    {
        using var channel = await _connectionManager.CreateChannelAsync();

        foreach (var queueConfig in exchangeConfig.Queues)
        {
            // Declare queue
            await channel.QueueDeclareAsync(
                queue: queueConfig.Name,
                durable: queueConfig.Durable,
                exclusive: queueConfig.Exclusive,
                autoDelete: queueConfig.AutoDelete,
                arguments: queueConfig.Arguments,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Declared queue {Name}", queueConfig.Name);

            // Bind queue to appropriate exchange
            var exchangeName = MessagingConfiguration.GetExchangeForQueueType(exchangeConfig, queueConfig.Type);

            await channel.QueueBindAsync(
                queue: queueConfig.Name,
                exchange: exchangeName,
                routingKey: queueConfig.RoutingKey,
                arguments: null,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Bound queue {Name} to exchange {ExchangeName} with routing key {RoutingKey}", queueConfig.Name, exchangeName, queueConfig.RoutingKey);
        }
    }
}