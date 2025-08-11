namespace ConnectFlow.Infrastructure.Common.Models;

public class RabbitMQSettings
{
    public const string SectionName = "RabbitMQ";
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public bool UseSSL { get; set; } = false;
    public int PrefetchCount { get; set; } = 10;
    public int MaxConcurrentConsumers { get; set; } = 5;
}