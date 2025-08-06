namespace ConnectFlow.Domain.Constants;

public static class MessagingConfiguration
{
    public class Exchange
    {
        public string DomainEvents { get; private set; } = string.Empty;
        public string DeadLetter { get; private set; } = string.Empty;
        public string Retry { get; private set; } = string.Empty;
        public List<Queue> Queues { get; private set; } = new List<Queue>();

        public Exchange(string domainEvents, string deadLetter, string retry)
        {
            DomainEvents = domainEvents;
            DeadLetter = deadLetter;
            Retry = retry;
            Queues = new List<Queue>()
            {
            };
        }

        public Exchange AddQueues(string queueIdentifier, QueueDomain domain)
        {
            Queues.Add(new Queue
            {
                Name = $"{queueIdentifier}-queue",
                RoutingKey = $"event.{queueIdentifier}",
                Type = QueueType.Default,
                Domain = domain,
                Arguments = new Dictionary<string, object?>
                {
                    ["x-dead-letter-exchange"] = DeadLetter,
                    ["x-dead-letter-routing-key"] = $"event.{queueIdentifier}.dlx",
                    ["x-message-ttl"] = 86400000 // 24 hours
                }
            });

            Queues.Add(new Queue
            {
                Name = $"{queueIdentifier}-queue.dlx",
                RoutingKey = $"event.{queueIdentifier}.dlx",
                Type = QueueType.DeadLetter,
                Domain = domain,
                Arguments = new Dictionary<string, object?> { }
            });

            Queues.Add(new Queue
            {
                Name = $"{queueIdentifier}-queue.retry",
                RoutingKey = $"event.{queueIdentifier}.retry",
                Type = QueueType.Retry,
                Domain = domain,
                Arguments = new Dictionary<string, object?>
                {
                    ["x-message-ttl"] = 30000, // 30 seconds
                    ["x-dead-letter-exchange"] = DomainEvents,
                    ["x-dead-letter-routing-key"] = $"event.{queueIdentifier}"
                }
            });

            return this;
        }
    }

    public class Queue
    {
        public string Name { get; init; } = string.Empty;
        public string RoutingKey { get; init; } = string.Empty;
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public int MaxRetries { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 30;
        public QueueType Type { get; init; } = QueueType.Default;
        public QueueDomain Domain { get; init; } = QueueDomain.None;
        public Dictionary<string, object?> Arguments { get; init; } = new Dictionary<string, object?>();
    }

    public enum QueueType
    {
        Default,
        DeadLetter,
        Retry
    }

    public enum QueueDomain
    {
        None,
        Email,
    }

    private static readonly Exchange _exchange = new Exchange("domain-events-exchange", "dead-letter-exchange", "retry-exchange").AddQueues("email", QueueDomain.Email);

    public static Exchange GetExchange() => _exchange;

    public static string DefaultExchangeName => "domain-events-exchange";

    public static string GetExchangeForQueueType(Exchange exchangeConfig, QueueType queueType)
    {
        return queueType switch
        {
            QueueType.DeadLetter => exchangeConfig.DeadLetter,
            QueueType.Retry => exchangeConfig.Retry,
            _ => exchangeConfig.DomainEvents
        };
    }

    public static Queue GetQueueByTypeAndDomain(QueueType queueType, QueueDomain domain)
    {
        return GetExchange().Queues.FirstOrDefault(q => q.Type == queueType && q.Domain == domain) ?? throw new InvalidOperationException($"Queue of type {queueType} and domain {domain} not found.");
    }
}