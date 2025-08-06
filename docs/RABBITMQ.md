# RabbitMQ in ConnectFlow

This document provides a comprehensive overview of the RabbitMQ implementation in the ConnectFlow application, including recent improvements, configuration details, monitoring setup, and guidance for extending the messaging system.

## Table of Contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Configuration](#configuration)
4. [Recent Improvements](#recent-improvements)
5. [Monitoring](#monitoring)
6. [Creating New Queues](#creating-new-queues)
7. [Creating New Consumers](#creating-new-consumers)
8. [Error Handling & Retry Strategy](#error-handling--retry-strategy)
9. [Best Practices](#best-practices)
10. [Using the New Features](#using-the-new-features)

## Overview

ConnectFlow uses RabbitMQ as a message broker for asynchronous communication between system components. The implementation follows Clean Architecture principles, with a clear separation between messaging abstractions in the Domain and Application layers, and concrete RabbitMQ implementations in the Infrastructure layer.

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
5. `MessagingConfiguration`: Defines exchanges, queues, and bindings using a domain-driven approach.

## Configuration

### RabbitMQ Settings

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

### Messaging Configuration

The messaging system is configured through the `MessagingConfiguration` class in the Domain layer. This class provides a structured way to define exchanges, queues, and their relationships:

```csharp
// Example of the messaging configuration structure
public static class MessagingConfiguration
{
    public class Exchange
    {
        public string DomainEvents { get; private set; }
        public string DeadLetter { get; private set; }
        public string Retry { get; private set; }
        public List<Queue> Queues { get; private set; }
        
        // Methods to add queues
        public Exchange AddQueues(string queueIdentifier, QueueDomain domain) { ... }
    }
    
    public class Queue
    {
        public string Name { get; init; }
        public string RoutingKey { get; init; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public int MaxRetries { get; set; }
        public int RetryDelaySeconds { get; set; }
        public QueueType Type { get; init; }
        public QueueDomain Domain { get; init; }
        public Dictionary<string, object?> Arguments { get; init; }
    }
    
    // Enums for categorizing queues
    public enum QueueType { Default, DeadLetter, Retry }
    public enum QueueDomain { None, Email, /* other domains */ }
    
    // Helper methods
    public static Exchange GetExchange() => _exchange;
    public static string GetExchangeForQueueType(Exchange exchangeConfig, QueueType queueType) { ... }
    public static Queue GetQueueByTypeAndDomain(QueueType queueType, QueueDomain domain) { ... }
}
```

## Recent Improvements

### Domain-Driven Queue Management

- **Structured Configuration**: Implemented a more structured approach to queue configuration with dedicated classes
- **Domain Separation**: Introduced `QueueDomain` enum to categorize queues by business domain
- **Type Classification**: Added `QueueType` enum to distinguish between main queues, retry queues, and dead letter queues
- **Fluent Interface**: Added a fluent interface for adding new queues with proper configurations

### Monitoring Enhancements

- **RabbitMQ Exporter Integration**: Added the RabbitMQ Prometheus Exporter to export RabbitMQ metrics
- **Grafana Dashboard**: Created a comprehensive RabbitMQ dashboard in Grafana

### Application-Level Metrics

- **Message Tracking**: Implemented metrics for tracking published, consumed, failed, and retried messages
- **OpenTelemetry Integration**: Connected RabbitMQ metrics to OpenTelemetry for better observability

### Resilience Improvements

- **Enhanced Health Checks**: Added health checks for RabbitMQ connections
- **Better Error Logging**: Improved error logging with contextual information
- **Queue Depth Monitoring**: Added metrics for monitoring queue depths and message rates

## Monitoring

The RabbitMQ implementation includes comprehensive monitoring via:

1. **Health Checks**: RabbitMQ connection health is monitored through the ASP.NET Core health checks system.
2. **Metrics**: Application metrics are collected for messages published, consumed, failed, retried, and dead-lettered.
3. **Grafana Dashboard**: A pre-configured dashboard displays RabbitMQ metrics, queue stats, and connection status.
4. **Prometheus Integration**: RabbitMQ metrics are exposed via the RabbitMQ Prometheus exporter.

To access the monitoring:

- Grafana Dashboard: [http://localhost:3000/d/rabbitmq-dashboard](http://localhost:3000/d/rabbitmq-dashboard)
- RabbitMQ Management UI: [http://localhost:15672](http://localhost:15672) (guest/guest)
- Health Check UI: [http://localhost:5010/healthz](http://localhost:5010/healthz) (depends on your app port)

## Creating New Queues

With the new configuration system, adding new queues is significantly easier:

1. Add a new domain to the `QueueDomain` enum if needed:

```csharp
public enum QueueDomain
{
    None,
    Email,
    Notification,  // New domain
    // Add other domains as needed
}
```

2. Update the `_exchange` initialization to include your new queues:

```csharp
private static readonly Exchange _exchange = new Exchange(
    "domain-events-exchange", 
    "dead-letter-exchange", 
    "retry-exchange")
        .AddQueues("email", QueueDomain.Email)
        .AddQueues("notification", QueueDomain.Notification);  // Add new queue set
```

The `AddQueues` method automatically creates the main queue, dead letter queue, and retry queue with appropriate configurations.

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

2. Create a message handler in the Application layer:

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

3. Implement a consumer service in the Infrastructure layer:

```csharp
namespace ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ.Consumers;

public class YourNewEventConsumer : RabbitMQConsumerService<YourNewEvent>
{
    public YourNewEventConsumer(
        IRabbitMQConnectionManager connectionManager,
        IOptions<RabbitMQSettings> settings,
        IServiceProvider serviceProvider,
        ILogger<YourNewEventConsumer> logger)
        : base(connectionManager, settings, serviceProvider, logger)
    {
    }
    
    protected override Queue GetQueue()
    {
        return MessagingConfiguration.GetQueueByTypeAndDomain(
            QueueType.Default, 
            QueueDomain.YourDomain);
    }
    
    protected override Queue GetRetryQueue()
    {
        return MessagingConfiguration.GetQueueByTypeAndDomain(
            QueueType.Retry, 
            QueueDomain.YourDomain);
    }
}
```

4. Register the consumer in `DependencyInjection.cs`:

```csharp
// Register message handler
builder.Services.AddScoped<IMessageHandler<YourNewEvent>, YourNewEventHandler>();

// Register consumer as hosted service
builder.Services.AddHostedService<YourNewEventConsumer>();
```

5. Publish messages using the `IMessagePublisher`:

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
        
        var queue = MessagingConfiguration.GetQueueByTypeAndDomain(
            QueueType.Default, 
            QueueDomain.YourDomain);
        
        await _messagePublisher.PublishAsync(
            message, 
            MessagingConfiguration.DefaultExchangeName,
            queue.RoutingKey, 
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

The new configuration system automatically sets up the appropriate dead letter and retry queue configurations, ensuring consistent error handling across different message types.

## Best Practices

1. **Message Idempotency**: Design message handlers to be idempotent.
2. **Use Domain-Driven Design**: Organize queues by business domain using the `QueueDomain` enum.
3. **Targeted Routing Keys**: Use specific routing keys for targeted message delivery.
4. **Proper Error Handling**: Handle exceptions in message handlers and decide whether to retry or reject.
5. **Message Serialization**: Keep message payloads reasonably sized and well-structured.
6. **Monitoring**: Regularly check RabbitMQ dashboards for queue sizes and error rates.
7. **Consumer Scaling**: Adjust `MaxConcurrentConsumers` based on message throughput.
8. **Connection Pooling**: The connection manager reuses connections and channels when possible.
9. **Transaction Management**: Use transactions for operations that must be atomic.

## Using the New Features

### Viewing RabbitMQ Metrics

1. Start the Docker environment with `docker-compose up -d`
2. Access the Grafana dashboard at [http://localhost:3000](http://localhost:3000)
3. Navigate to the RabbitMQ Dashboard
4. Monitor queues, connections, and message rates

### Health Monitoring

- RabbitMQ health is now included in the application health checks
- View health status at [http://localhost:5010/health](http://localhost:5010/health) (adjust port as needed)
- Health check UI available at [http://localhost:5010/healthz](http://localhost:5010/healthz)

### Using the Domain-Driven Configuration

The new configuration system allows you to:

1. **Retrieve Queue Information by Domain**:
```csharp
var emailQueue = MessagingConfiguration.GetQueueByTypeAndDomain(QueueType.Default, QueueDomain.Email);
```

2. **Get the Appropriate Exchange for Queue Types**:
```csharp
var exchange = MessagingConfiguration.GetExchange();
var deadLetterExchangeName = MessagingConfiguration.GetExchangeForQueueType(exchange, QueueType.DeadLetter);
```

3. **Access Complete Exchange Configuration**:
```csharp
var exchange = MessagingConfiguration.GetExchange();
foreach (var queue in exchange.Queues)
{
    Console.WriteLine($"Queue: {queue.Name}, Type: {queue.Type}, Domain: {queue.Domain}");
}
```

For more information, refer to the RabbitMQ documentation at [https://www.rabbitmq.com/documentation.html](https://www.rabbitmq.com/documentation.html)
