using System.Text;
using System.Text.Json;
using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Constants;
using RabbitMQ.Client;

namespace ConnectFlow.Infrastructure.Services.Messaging.RabbitMQ;

public class RabbitMQPublisher : IMessagePublisher, IDisposable
{
    private readonly IRabbitMQConnectionManager _connectionManager;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private readonly Metrics.RabbitMQMetrics _metrics;
    private IChannel? _channel;
    private bool _disposed;
    private readonly string _exchangeName = string.Empty;

    public RabbitMQPublisher(IRabbitMQConnectionManager connectionManager, ILogger<RabbitMQPublisher> logger, Metrics.RabbitMQMetrics metrics)
    {
        _connectionManager = connectionManager;
        _logger = logger;
        _metrics = metrics;

        _exchangeName = MessagingConfiguration.DefaultExchangeName;
    }

    public async Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : BaseMessageEvent
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(routingKey);

        try
        {
            var channel = await GetChannelAsync();
            var body = SerializeMessage(message);
            var properties = CreateBasicProperties(channel, message);

            await channel.BasicPublishAsync(
                exchange: _exchangeName,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogDebug("Published message {MessageType} with ID {MessageId} to exchange {Exchange} with routing key {RoutingKey}", typeof(T).Name, message.MessageId, _exchangeName, routingKey);
            _metrics.IncrementPublishedMessages(routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message {MessageType} with routing key {RoutingKey}",
                typeof(T).Name, routingKey);
            throw;
        }
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> messages, string routingKey, CancellationToken cancellationToken = default) where T : BaseMessageEvent
    {
        ArgumentNullException.ThrowIfNull(messages);
        ArgumentException.ThrowIfNullOrWhiteSpace(routingKey);

        var messageList = messages.ToList();
        if (!messageList.Any())
            return;

        try
        {
            var channel = await GetChannelAsync();

            foreach (var message in messageList)
            {
                var body = SerializeMessage(message);
                var properties = CreateBasicProperties(channel, message);

                await channel.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);
            }

            _logger.LogDebug("Published batch of {Count} messages of type {MessageType} with routing key {RoutingKey}", messageList.Count, typeof(T).Name, routingKey);

            // Track metrics for each message published
            foreach (var _ in messageList)
            {
                _metrics.IncrementPublishedMessages(routingKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish batch of {Count} messages of type {MessageType} with routing key {RoutingKey}", messageList.Count, typeof(T).Name, routingKey);
            throw;
        }
    }

    private static byte[] SerializeMessage<T>(T message) where T : BaseMessageEvent
    {
        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return Encoding.UTF8.GetBytes(json);
    }

    private static BasicProperties CreateBasicProperties<T>(IChannel channel, T message) where T : BaseMessageEvent
    {
        var properties = new BasicProperties
        {
            MessageId = message.MessageId.ToString(),
            CorrelationId = message.CorrelationId.ToString(),
            Timestamp = new AmqpTimestamp(((DateTimeOffset)message.Timestamp).ToUnixTimeSeconds()),
            ContentType = "application/json",
            ContentEncoding = "utf-8",
            DeliveryMode = DeliveryModes.Persistent,
            Headers = new Dictionary<string, object?>()
        };

        // Add custom headers
        properties.Headers["TenantId"] = message.TenantId;
        properties.Headers["ApplicationUserId"] = message.ApplicationUserId;
        properties.Headers["ApplicationUserPublicId"] = message.ApplicationUserPublicId.ToString();
        properties.Headers["MessageType"] = message.MessageType;
        properties.Headers["RetryCount"] = message.RetryCount;

        foreach (var header in message.Headers)
        {
            properties.Headers[header.Key] = header.Value;
        }

        return properties;
    }

    private async Task<IChannel> GetChannelAsync()
    {
        if (_channel?.IsOpen == true)
            return _channel;

        _channel?.Dispose();
        _channel = await _connectionManager.CreateChannelAsync();

        // Enable publisher confirms for reliability
        //await _channel.ConfirmSelectAsync();

        return _channel;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            _channel?.CloseAsync().GetAwaiter().GetResult();
            _channel?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ publisher");
        }
    }
}