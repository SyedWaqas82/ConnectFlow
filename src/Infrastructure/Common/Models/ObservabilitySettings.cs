namespace ConnectFlow.Infrastructure.Common.Models;

/// <summary>
/// Configuration settings for monitoring and observability
/// </summary>
public class ObservabilitySettings
{
    public bool UseLoki { get; set; } = false;
    public string LokiUrl { get; set; } = "http://loki:3100";
    public string OtlpEndpoint { get; set; } = "http://tempo:4317";
    public string PrometheusUrl { get; set; } = "http://prometheus:9090";
    public string GrafanaUrl { get; set; } = "http://grafana:3000";
}

/// <summary>
/// Configuration settings for health checks
/// </summary>
public class HealthChecksSettings
{
    public int EvaluationTimeInSeconds { get; set; } = 60;
    public int MinimumSecondsBetweenFailureNotifications { get; set; } = 300;
}