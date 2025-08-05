using System;
using Microsoft.Extensions.Hosting;

namespace ConnectFlow.Infrastructure.Common.Messaging.RabbitMQ;

public class RabbitMQSetupHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMQSetupHostedService> _logger;

    public RabbitMQSetupHostedService(IServiceProvider serviceProvider, ILogger<RabbitMQSetupHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting RabbitMQ setup...");

        using var scope = _serviceProvider.CreateScope();
        var setupService = scope.ServiceProvider.GetRequiredService<IRabbitMQSetupService>();

        try
        {
            await setupService.SetupTopologyAsync(cancellationToken);
            _logger.LogInformation("RabbitMQ setup completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup RabbitMQ topology");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ setup service stopped");
        return Task.CompletedTask;
    }
}