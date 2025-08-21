using System.Diagnostics.Metrics;
using System.Collections.Concurrent;
using RabbitMQ.Client;
using ConnectFlow.Infrastructure.Services.Messaging.RabbitMQ;
using ConnectFlow.Infrastructure.Common.Models;

namespace ConnectFlow.Infrastructure.Metrics;

public class RabbitMQMetrics : IDisposable
{
    private readonly Meter _meter;
    private readonly IRabbitMQConnectionManager _connectionManager;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQMetrics> _logger;

    // Track connections, channels and consumers
    private readonly ConcurrentDictionary<string, IConnection> _activeConnections = new();
    private readonly ConcurrentDictionary<string, IChannel> _activeChannels = new();
    private readonly ConcurrentDictionary<string, string> _activeConsumers = new();

    // Counters for tracking messages
    private readonly Counter<long> _publishedMessagesCounter;
    private readonly Counter<long> _consumedMessagesCounter;
    private readonly Counter<long> _failedMessagesCounter;
    private readonly Counter<long> _retryMessagesCounter;
    private readonly Counter<long> _deadLetterMessagesCounter;

    public RabbitMQMetrics(IRabbitMQConnectionManager connectionManager, IOptions<RabbitMQSettings> settings, ILogger<RabbitMQMetrics> logger)
    {
        _connectionManager = connectionManager;
        _settings = settings.Value;
        _logger = logger;

        // Create meter
        _meter = new Meter("ConnectFlow.RabbitMQ", "1.0.0");

        // Create counters
        _publishedMessagesCounter = _meter.CreateCounter<long>("rabbitmq.messages.published", "messages", "Total number of messages published");
        _consumedMessagesCounter = _meter.CreateCounter<long>("rabbitmq.messages.consumed", "messages", "Total number of messages consumed");
        _failedMessagesCounter = _meter.CreateCounter<long>("rabbitmq.messages.failed", "messages", "Total number of failed messages");
        _retryMessagesCounter = _meter.CreateCounter<long>("rabbitmq.messages.retry", "messages", "Total number of retried messages");
        _deadLetterMessagesCounter = _meter.CreateCounter<long>("rabbitmq.messages.deadletter", "messages", "Total number of dead-letter messages");

        // Create gauges for connection state and metrics
        _meter.CreateObservableGauge("rabbitmq.connection.state", () => _connectionManager.IsConnected ? 1 : 0, "state", "RabbitMQ connection state (1=connected, 0=disconnected)");
        _meter.CreateObservableGauge("rabbitmq.connections.active", () => _activeConnections.Count, "connections", "Number of active RabbitMQ connections");
        _meter.CreateObservableGauge("rabbitmq.channels.active", () => _activeChannels.Count, "channels", "Number of active RabbitMQ channels");
        _meter.CreateObservableGauge("rabbitmq.consumers.active", () => _activeConsumers.Count, "consumers", "Number of active RabbitMQ consumers");

        // Subscribe to connection events from the connection manager
        _connectionManager.Connected += OnConnectionEstablished;
        _connectionManager.Disconnected += OnConnectionClosed;
    }

    public void IncrementPublishedMessages(string routingKey)
    {
        _publishedMessagesCounter.Add(1, new KeyValuePair<string, object?>("routing_key", routingKey));
        _logger.LogTrace("Incremented published messages counter for routing key {RoutingKey}", routingKey);
    }

    public void IncrementConsumedMessages(string queueName, string messageType)
    {
        _consumedMessagesCounter.Add(1, new KeyValuePair<string, object?>("queue", queueName),
            new KeyValuePair<string, object?>("message_type", messageType));
        _logger.LogTrace("Incremented consumed messages counter for queue {QueueName} and message type {MessageType}", queueName, messageType);
    }

    public void IncrementFailedMessages(string queueName, string errorType)
    {
        _failedMessagesCounter.Add(1, new KeyValuePair<string, object?>("queue", queueName),
            new KeyValuePair<string, object?>("error_type", errorType));
        _logger.LogTrace("Incremented failed messages counter for queue {QueueName} with error {ErrorType}", queueName, errorType);
    }

    public void IncrementRetryMessages(string queueName)
    {
        _retryMessagesCounter.Add(1, new KeyValuePair<string, object?>("queue", queueName));
        _logger.LogTrace("Incremented retry messages counter for queue {QueueName}", queueName);
    }

    public void IncrementDeadLetterMessages(string queueName, string reason)
    {
        _deadLetterMessagesCounter.Add(1, new KeyValuePair<string, object?>("queue", queueName),
            new KeyValuePair<string, object?>("reason", reason));
        _logger.LogTrace("Incremented dead letter messages counter for queue {QueueName} with reason {Reason}", queueName, reason);
    }

    // Connection event handlers
    private void OnConnectionEstablished(object? sender, EventArgs e)
    {
        if (sender is IRabbitMQConnectionManager connectionManager)
        {
            try
            {
                var connection = connectionManager.GetConnectionAsync().GetAwaiter().GetResult();
                if (connection != null)
                {
                    string connectionId = connection.ClientProvidedName ?? $"conn_{Guid.NewGuid():N}";
                    _activeConnections.TryAdd(connectionId, connection);
                    _logger.LogDebug("RabbitMQ connection tracked: {ConnectionId}", connectionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking RabbitMQ connection");
            }
        }
    }

    private void OnConnectionClosed(object? sender, EventArgs e)
    {
        // Remove all connections, channels and consumers when connection is lost
        _activeConnections.Clear();
        _activeChannels.Clear();
        _activeConsumers.Clear();
        _logger.LogDebug("Cleared all tracked RabbitMQ connections, channels and consumers due to disconnect");
    }

    // Methods to track channels and consumers
    public void TrackChannel(IChannel channel)
    {
        if (channel == null) return;

        string channelId = $"channel_{Guid.NewGuid():N}";
        _activeChannels.TryAdd(channelId, channel);

        // Set up removal when channel is closed
        channel.ChannelShutdownAsync += async (sender, args) =>
        {
            _activeChannels.TryRemove(channelId, out _);
            _logger.LogDebug("RabbitMQ channel untracked: {ChannelId}", channelId);
            await Task.CompletedTask;
        };

        _logger.LogDebug("RabbitMQ channel tracked: {ChannelId}", channelId);
    }

    public void TrackConsumer(string consumerTag, string queueName)
    {
        if (string.IsNullOrEmpty(consumerTag)) return;

        _activeConsumers.TryAdd(consumerTag, queueName);
        _logger.LogDebug("RabbitMQ consumer tracked: {ConsumerTag} for queue {QueueName}", consumerTag, queueName);
    }

    public void UntrackConsumer(string consumerTag)
    {
        if (string.IsNullOrEmpty(consumerTag)) return;

        _activeConsumers.TryRemove(consumerTag, out _);
        _logger.LogDebug("RabbitMQ consumer untracked: {ConsumerTag}", consumerTag);
    }

    public void Dispose()
    {
        // Unsubscribe from connection events
        if (_connectionManager != null)
        {
            _connectionManager.Connected -= OnConnectionEstablished;
            _connectionManager.Disconnected -= OnConnectionClosed;
        }

        _activeConnections.Clear();
        _activeChannels.Clear();
        _activeConsumers.Clear();
        _meter?.Dispose();
    }
}
