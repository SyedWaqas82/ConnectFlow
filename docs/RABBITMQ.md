# RabbitMQ Setup in ConnectFlow

This document provides an overview of the RabbitMQ messaging system implementation in the ConnectFlow application. It explains the core components, configuration, monitoring, and instructions for creating new queues and consumers.

## Table of Contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Configuration](#configuration)
4. [Monitoring](#monitoring)
5. [Creating New Queues](#creating-new-queues)
6. [Creating New Consumers](#creating-new-consumers)
7. [Error Handling & Retry Strategy](#error-handling--retry-strategy)
8. [Best Practices](#best-practices)

## Overview

ConnectFlow uses RabbitMQ as a message broker for asynchronous communication between system components. The implementation follows the Clean Architecture principles, with a separation between the messaging abstractions and the concrete RabbitMQ implementation.

## Architecture

The RabbitMQ implementation consists of these key components:

- **Connection Management**: Handles establishing and maintaining connections to RabbitMQ.
- **Message Publishing**: Responsible for sending messages to RabbitMQ.
- **Message Consumption**: Background services that receive and process messages.
- **Topology Setup**: Configures exchanges, queues, and bindings.
- **Error Handling**: Implements retry logic and dead letter queues.

### Core Components

1. `RabbitMQConnectionManager`: Manages the connection to RabbitMQ with automatic reconnection.
2. `RabbitMQPublisher`: Handles message publishing to RabbitMQ.
3. `RabbitMQConsumerService`: Base class for message consumers.
4. `RabbitMQSetupService`: Configures the RabbitMQ topology (exchanges, queues, bindings).
5. `MessagingConfiguration`: Defines exchanges, queues, and bindings.

## Configuration

RabbitMQ settings are defined in the `RabbitMQSettings` class and configured in `appsettings.json`:

```json
"RabbitMQ": {
  "HostName": "localhost",
  "Port": 5672,
  "UserName": "guest",
  "Password": "guest",
  "VirtualHost": "/",
  "UseSSL": false,
  "ConnectionRetryCount": 5,
  "ConnectionRetryDelay": 2000,
  "PrefetchCount": 10,
  "MaxConcurrentConsumers": 5,
  "ExchangeName": "domain-events-exchange",
  "DeadLetterExchangeName": "dead-letter-exchange",
  "MessageTTL": 86400000,
  "RetryLimit": 3
}
```

## Monitoring

The RabbitMQ implementation includes monitoring via:

1. **Health Checks**: RabbitMQ connection health is monitored through the ASP.NET Core health checks system.
2. **Metrics**: Application metrics are collected for messages published, consumed, failed, retried, and dead-lettered.
3. **Grafana Dashboard**: A pre-configured dashboard displays RabbitMQ metrics, queue stats, and connection status.
4. **Prometheus Integration**: RabbitMQ metrics are exposed via the RabbitMQ Prometheus exporter.

To access the monitoring:

- Grafana Dashboard: [http://localhost:3000/d/rabbitmq-dashboard](http://localhost:3000/d/rabbitmq-dashboard)
- RabbitMQ Management UI: [http://localhost:15672](http://localhost:15672) (guest/guest)
- Health Check UI: [http://localhost:5010/healthz](http://localhost:5010/healthz) (depends on your app port)

## Creating New Queues

To add a new queue to the system:

1. Define the queue in `MessagingConfiguration.cs`:

```csharp
public static class Queues
{
    // Existing queues
    public const string Email = "email-queue";
    public const string EmailDeadLetter = "email-queue.dlx";
    public const string EmailRetry = "email-queue.retry";
    
    // Add new queues
    public const string YourNewQueue = "your-new-queue";
    public const string YourNewQueueDeadLetter = "your-new-queue.dlx";
    public const string YourNewQueueRetry = "your-new-queue.retry";
}

public static class RoutingKeys
{
    // Existing routing keys
    public const string Email = "event.email";
    public const string EmailRetry = "event.email.retry";
    
    // Add new routing keys
    public const string YourNewEvent = "event.your-new-event";
    public const string YourNewEventRetry = "event.your-new-event.retry";
}
```

1. Add the queue configuration in the `GetQueueConfigurations` method:

```csharp
public static Dictionary<string, QueueConfiguration> GetQueueConfigurations()
{
    return new Dictionary<string, QueueConfiguration>
    {
        // Existing configurations
        [Queues.Email] = new QueueConfiguration { ... },
        
        // Add your new queue
        [Queues.YourNewQueue] = new QueueConfiguration
        {
            QueueName = Queues.YourNewQueue,
            RoutingKey = RoutingKeys.YourNewEvent,
            Arguments = new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = Exchanges.DeadLetter,
                ["x-dead-letter-routing-key"] = $"{RoutingKeys.YourNewEvent}.dlx",
                ["x-message-ttl"] = 86400000 // 24 hours
            }
        },
        [Queues.YourNewQueueDeadLetter] = new QueueConfiguration
        {
            QueueName = Queues.YourNewQueueDeadLetter,
            RoutingKey = $"{RoutingKeys.YourNewEvent}.dlx"
        },
        [Queues.YourNewQueueRetry] = new QueueConfiguration
        {
            QueueName = Queues.YourNewQueueRetry,
            RoutingKey = RoutingKeys.YourNewEventRetry,
            Arguments = new Dictionary<string, object?>
            {
                ["x-message-ttl"] = 30000, // 30 seconds
                ["x-dead-letter-exchange"] = Exchanges.DomainEvents,
                ["x-dead-letter-routing-key"] = RoutingKeys.YourNewEvent
            }
        }
    };
}
```

## Creating New Consumers

To create a new message consumer:

1. First, define your message event in the Domain layer:

```csharp
namespace ConnectFlow.Domain.Events.YourEvents;

public class YourNewEvent : MessageBaseEvent
{
    // Your message properties
    public string SomeProperty { get; set; } = string.Empty;
    
    public YourNewEvent() : base() 
    {
        MessageType = nameof(YourNewEvent);
    }
}
```

1. Create a message handler in the Application layer:

```csharp
namespace ConnectFlow.Application.YourFeature.EventHandlers;

public class YourNewEventHandler : IMessageHandler<YourNewEvent>
{
    private readonly IYourService _yourService;
    private readonly ILogger<YourNewEventHandler> _logger;
    
    public YourNewEventHandler(IYourService yourService, ILogger<YourNewEventHandler> logger)
    {
        _yourService = yourService;
        _logger = logger;
    }
    
    public async Task HandleAsync(YourNewEvent message, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing {MessageType} with ID {MessageId}", 
                nameof(YourNewEvent), message.MessageId);
            
            // Process your message
            await _yourService.ProcessSomethingAsync(message.SomeProperty, cancellationToken);
            
            // Mark as acknowledged
            message.Acknowledge();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing {MessageType} with ID {MessageId}",
                nameof(YourNewEvent), message.MessageId);
            
            // Mark as rejected, and allow requeue
            message.Reject(requeue: true);
            throw;
        }
    }
}
```

1. Implement a consumer service in the Infrastructure layer:

```csharp
namespace ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ.Consumers;

public class YourNewEventConsumer : RabbitMQConsumerService<YourNewEvent>
{
    public YourNewEventConsumer(
        IRabbitMQConnectionManager connectionManager,
        IOptions<RabbitMQSettings> settings,
        IServiceProvider serviceProvider,
        ILogger<YourNewEventConsumer> logger)
        : base(connectionManager, settings, serviceProvider, logger, MessagingConfiguration.Queues.YourNewQueue)
    {
    }
    
    protected override string GetRetryQueueName() => MessagingConfiguration.Queues.YourNewQueueRetry;
    protected override string GetRetryRoutingKey() => MessagingConfiguration.RoutingKeys.YourNewEventRetry;
}
```

1. Register the consumer in `DependencyInjection.cs`:

```csharp
// Register message handler
builder.Services.AddScoped<IMessageHandler<YourNewEvent>, YourNewEventHandler>();

// Register consumer as hosted service
builder.Services.AddHostedService<YourNewEventConsumer>();
```

1. Publish messages using the `IMessagePublisher`:

```csharp
public class YourService
{
    private readonly IMessagePublisher _messagePublisher;
    
    public YourService(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }
    
    public async Task SendSomeEventAsync(string someProperty, CancellationToken cancellationToken)
    {
        var message = new YourNewEvent
        {
            SomeProperty = someProperty,
            // Base properties are already set in constructor
        };
        
        await _messagePublisher.PublishAsync(
            message, 
            MessagingConfiguration.RoutingKeys.YourNewEvent, 
            cancellationToken);
    }
}
```

## Error Handling & Retry Strategy

The system implements a resilient messaging approach:

1. **Automatic Reconnection**: The connection manager handles connection failures and automatically reconnects.
2. **Message Acknowledgment**: Messages are explicitly acknowledged or rejected.
3. **Retry Queues**: Failed messages are sent to retry queues with a delay.
4. **Dead Letter Queues**: Messages that exceed the retry limit are sent to dead letter queues.
5. **Metrics Collection**: Message failures are tracked for monitoring.

## Best Practices

1. **Message Idempotency**: Design message handlers to be idempotent.
2. **Targeted Routing Keys**: Use specific routing keys for targeted message delivery.
3. **Proper Error Handling**: Handle exceptions in message handlers and decide whether to retry or reject.
4. **Message Serialization**: Keep message payloads reasonably sized and well-structured.
5. **Monitoring**: Regularly check RabbitMQ dashboards for queue sizes and error rates.
6. **Consumer Scaling**: Adjust `MaxConcurrentConsumers` based on message throughput.
7. **Connection Pooling**: The connection manager reuses connections and channels when possible.
8. **Transaction Management**: Use transactions for operations that must be atomic.

For more information, refer to the RabbitMQ documentation at [https://www.rabbitmq.com/documentation.html](https://www.rabbitmq.com/documentation.html)
