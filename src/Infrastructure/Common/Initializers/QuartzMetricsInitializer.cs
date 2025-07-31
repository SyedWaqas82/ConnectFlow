using ConnectFlow.Infrastructure.Common.Providers;
using Microsoft.Extensions.Hosting;

namespace ConnectFlow.Infrastructure.Common.Initializers;

/// <summary>
/// Simple hosted service that initializes Quartz metrics once at application startup
/// </summary>
public class QuartzMetricsInitializer : IHostedService
{
    private readonly QuartzMetricsProvider _metricsProvider;

    public QuartzMetricsInitializer(QuartzMetricsProvider metricsProvider)
    {
        _metricsProvider = metricsProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _metricsProvider.InitializeAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}