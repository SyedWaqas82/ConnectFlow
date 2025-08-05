namespace ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ.Configurations;

public class RabbitMQSettings
{
    public const string SectionName = "RabbitMQ";

    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public bool UseSSL { get; set; } = false;
    public int ConnectionRetryCount { get; set; } = 5;
    public int ConnectionRetryDelay { get; set; } = 2000;
    public int PrefetchCount { get; set; } = 10;
    public int MaxConcurrentConsumers { get; set; } = 5;
    public string ExchangeName { get; set; } = "domain-events-exchange";
    public string DeadLetterExchangeName { get; set; } = "dead-letter-exchange";
    public int MessageTTL { get; set; } = 86400000; // 24 hours in milliseconds
    public int RetryLimit { get; set; } = 3;
}