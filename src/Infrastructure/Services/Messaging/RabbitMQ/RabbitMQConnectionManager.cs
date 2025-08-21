using System.Collections.Concurrent;
using ConnectFlow.Infrastructure.Common.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConnectFlow.Infrastructure.Services.Messaging.RabbitMQ;

public class RabbitMQConnectionManager : IRabbitMQConnectionManager
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQConnectionManager> _logger;
    private readonly ConcurrentBag<IChannel> _channels = new();
    private IConnection? _connection;
    private readonly object _lock = new();
    private bool _disposed;

    public bool IsConnected => _connection?.IsOpen == true;
    public event EventHandler<EventArgs>? Connected;
    public event EventHandler<EventArgs>? Disconnected;

    public RabbitMQConnectionManager(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQConnectionManager> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<IConnection> GetConnectionAsync()
    {
        if (IsConnected)
            return _connection!;

        bool needToCreateConnection = false;
        lock (_lock)
        {
            if (!IsConnected)
            {
                needToCreateConnection = true;
            }
        }

        if (needToCreateConnection)
        {
            await CreateConnectionAsync();
        }

        return _connection!;
    }

    public async Task<IChannel> CreateChannelAsync()
    {
        var connection = await GetConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        _channels.Add(channel);
        SetupChannelEvents(channel);

        return channel;
    }

    private async Task CreateConnectionAsync()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                TopologyRecoveryEnabled = true,
                ConsumerDispatchConcurrency = 1 // RabbitMQ.Client 7.x
            };

            if (_settings.UseSSL)
            {
                factory.Ssl = new SslOption
                {
                    Enabled = true,
                    ServerName = _settings.HostName
                };
            }

            _connection = await factory.CreateConnectionAsync($"App-{Environment.MachineName}-{Guid.NewGuid():N}");

            _connection.ConnectionShutdownAsync += OnConnectionShutdown;
            _connection.ConnectionBlockedAsync += OnConnectionBlocked;
            _connection.ConnectionUnblockedAsync += OnConnectionUnblocked;
            _connection.CallbackExceptionAsync += OnCallbackException;

            _logger.LogInformation("RabbitMQ connection established to {HostName}:{Port}",
                _settings.HostName, _settings.Port);

            Connected?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create RabbitMQ connection");
            throw;
        }
    }

    private void SetupChannelEvents(IChannel channel)
    {
        channel.ChannelShutdownAsync += async (sender, args) =>
        {
            _logger.LogWarning("RabbitMQ channel shutdown: {ReplyText}", args.ReplyText);

            await Task.CompletedTask; // Placeholder for potential async work
        };

        channel.BasicReturnAsync += async (sender, args) =>
        {
            _logger.LogWarning("RabbitMQ message returned: {ReplyText}, Exchange: {Exchange}, RoutingKey: {RoutingKey}", args.ReplyText, args.Exchange, args.RoutingKey);

            await Task.CompletedTask; // Placeholder for potential async work
        };
    }

    private async Task OnConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection shutdown: {ReplyText}", e.ReplyText);
        Disconnected?.Invoke(this, EventArgs.Empty);

        await Task.CompletedTask; // Placeholder for potential async work
    }

    private async Task OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection blocked: {Reason}", e.Reason);

        await Task.CompletedTask; // Placeholder for potential async work
    }

    private async Task OnConnectionUnblocked(object? sender, AsyncEventArgs e)
    {
        _logger.LogInformation("RabbitMQ connection unblocked");

        await Task.CompletedTask; // Placeholder for potential async work
    }

    private async Task OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "RabbitMQ callback exception");

        await Task.CompletedTask; // Placeholder for potential async work
    }

    public async void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            // Dispose all channels
            foreach (var channel in _channels.Where(c => c.IsOpen))
            {
                await channel.CloseAsync();
                channel.Dispose();
            }

            // Dispose connection
            if (_connection?.IsOpen == true)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }

            _logger.LogInformation("RabbitMQ connection manager disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ connection manager");
        }
    }
}