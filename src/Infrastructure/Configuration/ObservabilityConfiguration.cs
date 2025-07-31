using ConnectFlow.Application.Common.Models;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ConnectFlow.Infrastructure.Configuration;

public static class ObservabilityConfiguration
{
    /// <summary>
    /// Configures OpenTelemetry with Prometheus and OTLP exporters for unified observability
    /// </summary>
    /// <remarks>
    /// For complete documentation on the observability stack, configuration options, and 
    /// dashboard links, please see monitoring/README.md
    /// </remarks>
    public static void AddOpenTelemetry(this IHostApplicationBuilder builder)
    {
        var monitoringSettings = builder.Configuration.GetSection("Monitoring").Get<ObservabilitySettings>() ?? new ObservabilitySettings();
        var useOtlp = !string.IsNullOrEmpty(monitoringSettings.OtlpEndpoint);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(builder.Environment.ApplicationName)
                .AddTelemetrySdk())
            .WithTracing(tracing =>
            {
                ConfigureTracing(tracing, builder, monitoringSettings, useOtlp);
            })
            .WithMetrics(metrics =>
            {
                ConfigureMetrics(metrics);
            });

        // Configure settings in DI container for use elsewhere
        builder.Services.Configure<ObservabilitySettings>(builder.Configuration.GetSection("Monitoring"));
    }

    private static void ConfigureTracing(TracerProviderBuilder tracing, IHostApplicationBuilder builder, ObservabilitySettings monitoringSettings, bool useOtlp)
    {
        tracing.AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnrichWithHttpRequest = (activity, request) =>
                {
                    var tenantHeader = builder.Configuration["TenantSettings:HeaderName"] ?? "X-Tenant-Id";
                    activity.SetTag("tenant.id", request.Headers[tenantHeader].FirstOrDefault() ?? "default");
                    activity.SetTag("endpoint", request.Path);

                    if (request.Headers.TryGetValue("X-Correlation-Id", out var correlationId))
                    {
                        activity.SetTag("correlation.id", correlationId);
                    }
                };
            })
            .AddHttpClientInstrumentation()
            .AddSource("ConnectFlow");

        if (useOtlp)
        {
            tracing.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(monitoringSettings.OtlpEndpoint);
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });

            if (builder.Environment.IsDevelopment())
            {
                tracing.AddConsoleExporter();
            }
        }
        else
        {
            tracing.AddConsoleExporter();
        }
    }

    private static void ConfigureMetrics(MeterProviderBuilder metrics)
    {
        metrics.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter()
            .AddMeter("ConnectFlow.Metrics");
    }
}