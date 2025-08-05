using System.Text;
using System.Text.Json;
using ConnectFlow.Application.Common.Messaging;
using ConnectFlow.Domain.Common;
using ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ.Configurations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ;

public abstract class RabbitMQConsumerService<T> : BackgroundService, IMessageConsumer where T : MessageBaseEvent
{
    private readonly IRabbitMQConnectionManager _connectionManager;
    private readonly RabbitMQSettings _settings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMQConsumerService<T>> _logger;
    private readonly string _queueName;
    private readonly SemaphoreSlim _semaphore;
    private readonly Metrics.RabbitMQMetrics? _metrics;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;

    protected RabbitMQConsumerService(
        IRabbitMQConnectionManager connectionManager,
        IOptions<RabbitMQSettings> settings,
        IServiceProvider serviceProvider,
        ILogger<RabbitMQConsumerService<T>> logger,
        string queueName)
    {
        _connectionManager = connectionManager;
        _settings = settings.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _queueName = queueName;
        _semaphore = new SemaphoreSlim(_settings.MaxConcurrentConsumers, _settings.MaxConcurrentConsumers);
        _metrics = serviceProvider.GetService<Metrics.RabbitMQMetrics>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await StartConsumingAsync(stoppingToken);
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _channel = await _connectionManager.CreateChannelAsync();
            await _channel.BasicQosAsync(0, (ushort)_settings.PrefetchCount, false);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += HandleMessageAsync;

            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: _consumer,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Started consuming messages from queue {QueueName}", _queueName);

            // Keep the consumer running
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer for queue {QueueName} was cancelled", _queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in consumer for queue {QueueName}", _queueName);
            throw;
        }
    }

    public async Task StopConsumingAsync()
    {
        try
        {
            if (_consumer != null && _channel?.IsOpen == true)
            {
                await _channel.BasicCancelAsync(_consumer.ConsumerTags.FirstOrDefault() ?? string.Empty);
            }

            if (_channel != null)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }

            _logger.LogInformation("Stopped consuming messages from queue {QueueName}", _queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping consumer for queue {QueueName}", _queueName);
        }
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        await _semaphore.WaitAsync();

        try
        {
            var messageId = eventArgs.BasicProperties?.MessageId ?? "unknown";
            var correlationId = eventArgs.BasicProperties?.CorrelationId ?? "unknown";

            _logger.LogDebug("Processing message {MessageId} from queue {QueueName}", messageId, _queueName);

            var message = DeserializeMessage(eventArgs.Body.ToArray());

            if (message == null)
            {
                _logger.LogError("Failed to deserialize message payload {MessageId} from queue {QueueName}", messageId, _queueName);
                await _channel!.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetService<IMessageHandler<T>>();

            if (handler == null)
            {
                _logger.LogError("No handler found for message type {MessageType}", typeof(T).Name);
                await _channel!.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                return;
            }

            try
            {
                await handler.HandleAsync(message, CancellationToken.None);

                if (message.IsAcknowledged)
                {
                    await _channel!.BasicAckAsync(eventArgs.DeliveryTag, false);
                    _logger.LogDebug("Message {MessageId} processed successfully", messageId);
                    _metrics?.IncrementConsumedMessages(_queueName, message.MessageType ?? typeof(T).Name);
                }
                else if (message.IsRejected)
                {
                    if (ShouldRetry(message))
                    {
                        await PublishToRetryQueue(message, eventArgs);
                        await _channel!.BasicAckAsync(eventArgs.DeliveryTag, false);
                        _logger.LogWarning("Message {MessageId} sent to retry queue", messageId);
                        _metrics?.IncrementRetryMessages(_queueName);
                    }
                    else
                    {
                        await _channel!.BasicNackAsync(eventArgs.DeliveryTag, false, message.Requeue);
                        _logger.LogWarning("Message {MessageId} rejected after max retries", messageId);
                        _metrics?.IncrementDeadLetterMessages(_queueName, "max_retries_exceeded");
                    }
                }
                else
                {
                    // Handler didn't explicitly acknowledge or reject, default to ack
                    await _channel!.BasicAckAsync(eventArgs.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {MessageId} from queue {QueueName}", messageId, _queueName);

                if (ShouldRetry(message))
                {
                    await PublishToRetryQueue(message, eventArgs);
                    await _channel!.BasicAckAsync(eventArgs.DeliveryTag, false);
                    _metrics?.IncrementRetryMessages(_queueName);
                }
                else
                {
                    await _channel!.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                    _metrics?.IncrementDeadLetterMessages(_queueName, "exception_" + ex.GetType().Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing message from queue {QueueName}", _queueName);
            await _channel!.BasicNackAsync(eventArgs.DeliveryTag, false, false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static T? DeserializeMessage(byte[] body)
    {
        try
        {
            var json = Encoding.UTF8.GetString(body);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch
        {
            return null;
        }
    }

    private bool ShouldRetry(T message)
    {
        return message.RetryCount < _settings.RetryLimit;
    }

    private async Task PublishToRetryQueue(T message, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            message.RetryCount++;
            message.Timestamp = DateTime.UtcNow;

            var retryQueueName = GetRetryQueueName();
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

            var properties = new BasicProperties
            {
                MessageId = message.MessageId.ToString(),
                CorrelationId = message.CorrelationId,
                Timestamp = new AmqpTimestamp(((DateTimeOffset)message.Timestamp).ToUnixTimeSeconds()),
                ContentType = "application/json",
                ContentEncoding = "utf-8",
                DeliveryMode = DeliveryModes.Persistent,
                Headers = new Dictionary<string, object?>
                {
                    ["TenantId"] = message.TenantId,
                    ["UserId"] = message.UserId,
                    ["MessageType"] = message.MessageType,
                    ["RetryCount"] = message.RetryCount,
                    ["OriginalQueue"] = _queueName
                }
            };

            await _channel!.BasicPublishAsync(
                exchange: MessagingConfiguration.Exchanges.Retry,
                routingKey: GetRetryRoutingKey(),
                mandatory: true,
                basicProperties: properties,
                body: body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message {MessageId} to retry queue", message.MessageId);
        }
    }

    protected abstract string GetRetryQueueName();
    protected abstract string GetRetryRoutingKey();

    public override void Dispose()
    {
        _semaphore?.Dispose();
        _channel?.Dispose();
        base.Dispose();
    }
}