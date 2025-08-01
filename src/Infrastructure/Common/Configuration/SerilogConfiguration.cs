using ConnectFlow.Application.Common.Models;
using ConnectFlow.Infrastructure.Common.Enrichers;
using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Grafana.Loki;

namespace ConnectFlow.Infrastructure.Common.Configuration;

/// <summary>
/// Configures structured logging with Serilog for unified observability.
/// </summary>
/// <remarks>
/// For complete documentation on logging options, correlation with traces, and
/// dashboard links, please see monitoring/README.md
/// </remarks>
public static class SerilogConfiguration
{
    /// <summary>
    /// Adds structured logging to the application using Serilog.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    public static void AddStructuredLogging(this IHostApplicationBuilder builder)
    {
        // Load monitoring settings from configuration
        var monitoringSettings = builder.Configuration
            .GetSection("Monitoring")
            .Get<ObservabilitySettings>() ?? new ObservabilitySettings();

        // Create the base logger configuration
        var loggerConfiguration = CreateLoggerConfiguration();

        // Add Loki sink if enabled or in production
        if (ShouldUseLoki(builder, monitoringSettings))
        {
            AddLokiSink(loggerConfiguration, builder, monitoringSettings);
        }

        // Enrich logs with context if OTLP endpoint is set
        if (!string.IsNullOrEmpty(monitoringSettings.OtlpEndpoint))
        {
            loggerConfiguration.Enrich.FromLogContext();
        }

        // Set the global logger
        Log.Logger = loggerConfiguration.CreateLogger();
        builder.Services.AddSerilog(dispose: true);

        // Configure settings in DI container for use elsewhere
        builder.Services.Configure<ObservabilitySettings>(builder.Configuration.GetSection("Monitoring"));
    }

    /// <summary>
    /// Creates the base Serilog logger configuration with enrichers and sinks.
    /// </summary>
    private static LoggerConfiguration CreateLoggerConfiguration()
    {
        return new LoggerConfiguration()
            // Set minimum log levels for various namespaces
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Information)
            .MinimumLevel.Override("Quartz", LogEventLevel.Information)
            // Add enrichers for additional log context
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .Enrich.WithExceptionDetails()
            .Enrich.With<TelemetryEnricher>()
            .Enrich.WithProperty("Application", "ConnectFlow")
            .Enrich.WithProperty("ApplicationVersion", typeof(SerilogConfiguration).Assembly.GetName().Version?.ToString() ?? "1.0.0")
            // Write logs to console
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {CorrelationId}] [Tenant: {TenantId}] {ClientIP} {ClientId} {HttpMethod} {RequestPath} {StatusCode} {ResponseTimeMs}ms {Message:lj}{NewLine}{Exception}",
                theme: Serilog.Sinks.SystemConsole.Themes.SystemConsoleTheme.Literate)
            // Write logs to rolling file
            .WriteTo.File(
                path: "logs/connectflow-dev-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3} {CorrelationId}] [Tenant: {TenantId}] [{ClientIP}] [{ClientId}] [{HttpMethod} {RequestPath}] [{StatusCode}] [{ResponseTimeMs}ms] {Message:lj}{NewLine}{Exception}",
                fileSizeLimitBytes: 10485760,
                retainedFileCountLimit: 7)
            // Write error-level logs to a separate file
            .WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt => evt.Level >= LogEventLevel.Error)
                .WriteTo.File(
                    path: "logs/connectflow-dev-error-.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3} {CorrelationId}] [Tenant: {TenantId}] [{ClientIP}] [{ClientId}] [{HttpMethod} {RequestPath}] [{StatusCode}] [{ResponseTimeMs}ms] {Message:lj}{NewLine}{Exception}{NewLine}",
                    fileSizeLimitBytes: 10485760,
                    retainedFileCountLimit: 30));
    }

    /// <summary>
    /// Determines if the Loki sink should be used based on environment or settings.
    /// </summary>
    private static bool ShouldUseLoki(IHostApplicationBuilder builder, ObservabilitySettings monitoringSettings)
    {
        return builder.Environment.IsProduction() || monitoringSettings.UseLoki;
    }

    /// <summary>
    /// Adds the Grafana Loki sink to the logger configuration.
    /// </summary>
    private static void AddLokiSink(LoggerConfiguration loggerConfiguration, IHostApplicationBuilder builder, ObservabilitySettings monitoringSettings)
    {
        loggerConfiguration.WriteTo.GrafanaLoki(
            monitoringSettings.LokiUrl,
            labels: new[]
            {
                new LokiLabel { Key = "app", Value = "connectflow" },
                new LokiLabel { Key = "environment", Value = builder.Environment.EnvironmentName },
                new LokiLabel { Key = "instance", Value = Environment.MachineName }
            },
            propertiesAsLabels: new[]
            {
                "level",
                "service",
                "trace_id",
                "span_id",
                "CorrelationId",
                "TenantId",
                "ClientIP",
                "ClientId",
                "HttpMethod",
                "StatusCode",
                "ResponseTimeMs"
            });
    }
}