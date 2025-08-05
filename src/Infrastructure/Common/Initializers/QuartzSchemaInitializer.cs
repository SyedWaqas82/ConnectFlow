using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ConnectFlow.Infrastructure.Common.Providers;
using ConnectFlow.Infrastructure.Common.Models;

namespace ConnectFlow.Infrastructure.Common.Initializers;

/// <summary>
/// Hosted service responsible for initializing Quartz schema tables
/// after the application has started and migrations have completed
/// </summary>
public class QuartzSchemaInitializer : IHostedService
{
    private readonly QuartzSchemaProvider _schemaProvider;
    private readonly QuartzSettings _quartzSettings;

    public QuartzSchemaInitializer(QuartzSchemaProvider schemaProvider, IOptions<QuartzSettings> options)
    {
        _schemaProvider = schemaProvider;
        _quartzSettings = options.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_quartzSettings.UsePersistentStore)
        {
            _schemaProvider.EnsureTablesCreated();
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}