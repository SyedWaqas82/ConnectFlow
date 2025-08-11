using ConnectFlow.Infrastructure.Services.Messaging.RabbitMQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ConnectFlow.Infrastructure.Common.HealthChecks;

public class RabbitMQHealthCheck : IHealthCheck
{
    private readonly IRabbitMQConnectionManager _connectionManager;
    private readonly ILogger<RabbitMQHealthCheck> _logger;

    public RabbitMQHealthCheck(IRabbitMQConnectionManager connectionManager, ILogger<RabbitMQHealthCheck> logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_connectionManager.IsConnected)
            {
                return HealthCheckResult.Unhealthy("RabbitMQ connection is not established");
            }

            // Try to create and close a channel to verify connection health
            using var channel = await _connectionManager.CreateChannelAsync();
            if (!channel.IsOpen)
            {
                return HealthCheckResult.Unhealthy("Unable to create RabbitMQ channel");
            }

            return HealthCheckResult.Healthy("RabbitMQ is healthy");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ health check failed");
            return HealthCheckResult.Unhealthy("RabbitMQ health check failed", ex);
        }
    }
}