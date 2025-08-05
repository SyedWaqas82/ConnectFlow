using RabbitMQ.Client;

namespace ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ;

public interface IRabbitMQConnectionManager : IDisposable
{
    Task<IConnection> GetConnectionAsync();
    Task<IChannel> CreateChannelAsync();
    bool IsConnected { get; }
    event EventHandler<EventArgs>? Connected;
    event EventHandler<EventArgs>? Disconnected;
}