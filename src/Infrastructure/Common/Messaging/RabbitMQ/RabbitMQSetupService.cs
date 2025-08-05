using ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ.Configurations;
using RabbitMQ.Client;

namespace ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ;

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
        await EnsureQueuesAsync(cancellationToken);

        _logger.LogInformation("RabbitMQ topology setup completed");
    }

    public async Task EnsureExchangesAsync(CancellationToken cancellationToken = default)
    {
        using var channel = await _connectionManager.CreateChannelAsync();

        // Main domain events exchange
        await channel.ExchangeDeclareAsync(
            exchange: MessagingConfiguration.Exchanges.DomainEvents,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Declared exchange {ExchangeName}", MessagingConfiguration.Exchanges.DomainEvents);

        // Dead letter exchange
        await channel.ExchangeDeclareAsync(
            exchange: MessagingConfiguration.Exchanges.DeadLetter,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Declared exchange {ExchangeName}", MessagingConfiguration.Exchanges.DeadLetter);

        // Retry exchange
        await channel.ExchangeDeclareAsync(
            exchange: MessagingConfiguration.Exchanges.Retry,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Declared exchange {ExchangeName}", MessagingConfiguration.Exchanges.Retry);
    }

    public async Task EnsureQueuesAsync(CancellationToken cancellationToken = default)
    {
        using var channel = await _connectionManager.CreateChannelAsync();
        var queueConfigurations = MessagingConfiguration.GetQueueConfigurations();

        foreach (var (queueKey, config) in queueConfigurations)
        {
            // Declare queue
            await channel.QueueDeclareAsync(
                queue: config.QueueName,
                durable: config.Durable,
                exclusive: config.Exclusive,
                autoDelete: config.AutoDelete,
                arguments: config.Arguments,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Declared queue {QueueName}", config.QueueName);

            // Bind queue to appropriate exchange
            var exchangeName = GetExchangeForQueue(config.QueueName);

            await channel.QueueBindAsync(
                queue: config.QueueName,
                exchange: exchangeName,
                routingKey: config.RoutingKey,
                arguments: null,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Bound queue {QueueName} to exchange {ExchangeName} with routing key {RoutingKey}", config.QueueName, exchangeName, config.RoutingKey);
        }
    }

    private static string GetExchangeForQueue(string queueName)
    {
        return queueName switch
        {
            var name when name.EndsWith(".dlx") => MessagingConfiguration.Exchanges.DeadLetter,
            var name when name.EndsWith(".retry") => MessagingConfiguration.Exchanges.Retry,
            _ => MessagingConfiguration.Exchanges.DomainEvents
        };
    }
}
