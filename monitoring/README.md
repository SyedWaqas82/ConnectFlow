# ConnectFlow Unified Observability

This document provides a comprehensive guide to the observability stack in ConnectFlow.

## 1. Observability Stack Overview

ConnectFlow implements a modern unified observability stack:

- **Metrics**: Prometheus collects and stores time-series metrics
- **Logs**: Loki aggregates structured logs from all services
- **Traces**: Tempo provides distributed tracing functionality
- **Health Checks**: Monitors system component health status
- **Dashboards**: Grafana visualizes all observability data

## 2. Quick Start

### 2.1. Start the Observability Stack

```bash
cd /Users/WaQas/Desktop/Projects/ConnectFlow
docker-compose up -d prometheus grafana loki tempo
```

### 2.2. Start Your Application

```bash
cd /Users/WaQas/Desktop/Projects/ConnectFlow
dotnet run --project src/Web/Web.csproj
```

### 2.3. Generate Sample Traffic

```bash
curl -v http://localhost:5000/api/weatherforecast
```

## 3. Unified Observability Components

### 3.1. OpenTelemetry Integration

OpenTelemetry provides:

- Distributed tracing with context propagation
- Automatic instrumentation of ASP.NET Core and HttpClient
- Custom metrics collection via Meter API
- Correlation between traces, metrics, and logs

Configuration options:

```json
{
  "Monitoring": {
    "UseLoki": true,
    "LokiUrl": "http://loki:3100",
    "OtlpEndpoint": "http://tempo:4317",
    "PrometheusUrl": "http://prometheus:9090",
    "GrafanaUrl": "http://grafana:3000"
  }
}
```

### 3.2. Structured Logging (Serilog)

Features:

- Multiple output sinks: console, file, and Loki
- OpenTelemetry trace correlation with `trace_id` and `span_id`
- Environment-based filtering and log levels
- Exception enrichment and structured data

Log Locations:

- Development logs: `logs/connectflow-dev-.log`
- Error logs: `logs/connectflow-dev-error-.log`
- Grafana Loki: [View in Grafana](http://localhost:3000/explore?orgId=1&left={"datasource":"loki","queries":[{"refId":"A","expr":"{app=\"connectflow\"}"}]})

Example usage:

```csharp
// Structured logging with semantic properties
_logger.LogInformation("Processing order {OrderId} for {CustomerId}", order.Id, customer.Id);

// Adding custom context
using(LogContext.PushProperty("TransactionId", txId))
{
    _logger.LogInformation("Transaction started");
}

// Automatic trace correlation
// When using OpenTelemetry, trace_id and span_id are automatically included
```

### 3.3. Health Monitoring

Endpoints:

- Basic health: `/health`
- Liveness probe: `/health/live`
- Readiness probe: `/health/ready`
- Health UI dashboard: `/healthz`

Monitored Components:

- PostgreSQL database connectivity
- Redis cache availability
- API endpoint status
- Background job health

Kubernetes integration:

```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 80
  initialDelaySeconds: 10
  periodSeconds: 15
readinessProbe:
  httpGet:
    path: /health/ready
    port: 80
  initialDelaySeconds: 15
  periodSeconds: 30
```

### 3.4. Rate Limiting

Protection features:

- Global IP-based rate limiting
- Tenant-specific API limits
- Fixed window limiting algorithm
- Token bucket limiting algorithm

Metrics:

- Rate limit exceeded counts
- Request throttling
- Rejection rates

Configuration:

```json
{
  "RateLimiting": {
    "Enabled": true,
    "FixedWindow": {
      "PermitLimit": 100,
      "WindowSeconds": 60
    },
    "TokenBucket": {
      "TokenLimit": 100,
      "TokensPerPeriod": 20
    }
  }
}
```

### 3.5. Background Jobs (Quartz)

Features:

- Scheduled job execution with cron expressions
- Persistent job storage in PostgreSQL
- Clustered job execution for fault tolerance
- Enhanced metrics collection:
  - `quartz_jobs_executed_count`: Job execution counts with success/failure labels
  - `quartz_jobs_misfire_count`: Detects and counts jobs that missed their scheduled time
  - `quartz_jobs_error_count`: Tracks errors by job name
  - `quartz_job_execution_time_seconds`: Histogram of job execution durations
  - `quartz_scheduler_running`: Scheduler operational status (0/1)
  - `quartz_scheduler_jobs_total`: Total number of jobs registered
  - `quartz_scheduler_jobs_executing`: Currently executing job count
  - `quartz_scheduler_queue_depth`: Number of jobs waiting to execute
  - `quartz_scheduler_time_to_next_job_seconds`: Time until next job execution

Example job:

```csharp
[DisallowConcurrentExecution]
public class DataCleanupJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var jobType = context.MergedJobDataMap.GetString("JobType");
        // Implementation...
    }
}
```

## 4. Unified Dashboards & Data Access

### 4.1. Main Access Points

- **Grafana**: [http://localhost:3000](http://localhost:3000) (admin/admin)
- **Prometheus**: [http://localhost:9090](http://localhost:9090)
- **Application Metrics**: [http://localhost:5000/metrics](http://localhost:5000/metrics)

### 4.2. Custom Dashboards

#### API Performance Dashboard

- URL: [http://localhost:3000/d/cf-api-performance](http://localhost:3000/d/cf-api-performance)
- Metrics:
  - Request rates and latencies
  - Status code distribution
  - Error rates by endpoint
  - Authentication flows

#### .NET Runtime Dashboard

- URL: [http://localhost:3000/d/cf-dotnet-runtime](http://localhost:3000/d/cf-dotnet-runtime)
- Metrics:
  - Memory usage and GC statistics
  - CPU utilization
  - Thread pool metrics
  - HTTP request processing

#### Quartz Jobs Dashboard

- URL: [http://localhost:3000/d/cf-quartz-jobs](http://localhost:3000/d/cf-quartz-jobs)
- Metrics:
  - Job execution rates (success/failure)
  - Execution duration statistics (p50, p95)
  - Queue depth monitoring
  - Job misfire detection
  - Error counts by job
  - Success rate percentage
  - Scheduler running status
  - Time to next job execution

#### Rate Limiting Dashboard

- URL: [http://localhost:3000/d/cf-rate-limiting](http://localhost:3000/d/cf-rate-limiting)
- Metrics:
  - Request throttling events
  - Rejected request counts
  - Limit utilization by tenant
  - Token bucket metrics

### 4.3. Exploration Views

- **Metrics Explorer**: [http://localhost:3000/explore?orgId=1&left={"datasource":"prometheus"}](http://localhost:3000/explore?orgId=1&left={"datasource":"prometheus"})
- **Logs Explorer**: [http://localhost:3000/explore?orgId=1&left={"datasource":"loki"}](http://localhost:3000/explore?orgId=1&left={"datasource":"loki"})
- **Traces Explorer**: [http://localhost:3000/explore?orgId=1&left={"datasource":"tempo"}](http://localhost:3000/explore?orgId=1&left={"datasource":"tempo"})
- **Service Graph**: [http://localhost:3000/explore?orgId=1&left={"datasource":"tempo","queryType":"serviceMap"}](http://localhost:3000/explore?orgId=1&left={"datasource":"tempo","queryType":"serviceMap"})

## 5. Unified Data Correlation

### 5.1. Trace Context

All observability data is linked through:

- `trace_id`: Unique identifier for a request flow
- `span_id`: Identifier for a specific operation
- `X-Correlation-Id`: Custom correlation identifier
- `tenant.id`: Multi-tenancy context

### 5.2. Example Correlation Queries

Find logs for a specific trace:

```logql
{app="connectflow"} |= "trace_id=00112233445566778899aabbccddeeff"
```

Find all traces for an endpoint:

```logql
{endpoint="/api/v1/items"}
```

Find rate-limited requests:

```logql
{app="connectflow"} |= "rate limit exceeded"
```

Find slow background jobs:

```logql
{app="connectflow"} |= "job execution" |= "duration" | duration > 5s
```

### 5.3. Cross-Navigation

Grafana supports direct linking between:

- Metrics → Logs: Click data point to view related logs
- Logs → Traces: Click trace_id to view distributed trace
- Traces → Logs: View all logs related to a trace
- Service Graph: Visualize service dependencies

## 6. Troubleshooting

### 6.1. Common Issues

1. Missing metrics:
   - Check Prometheus targets: [http://localhost:9090/targets](http://localhost:9090/targets)
   - Verify metrics endpoint: `curl http://localhost:5000/metrics`
   - Check OpenTelemetry configuration

2. Missing logs:
   - Check Loki connection in Grafana
   - Verify Serilog configuration
   - Check log levels in configuration

3. Missing traces:
   - Verify OTLP endpoint configuration
   - Check Tempo ingestion status
   - Ensure trace sampling is enabled

### 6.2. Diagnostic Commands

```bash
# Check Prometheus targets
curl http://localhost:9090/targets

# Check application metrics
curl http://localhost:5000/metrics

# View container logs
docker-compose logs -f prometheus
docker-compose logs -f tempo
docker-compose logs -f loki

# Health check
curl http://localhost:5000/health
```

## 7. Best Practices

1. **Structured Logging**:
   - Use semantic logging with named properties
   - Include contextual information
   - Use appropriate log levels

2. **Distributed Tracing**:
   - Create spans for important operations
   - Add relevant tags to spans
   - Propagate trace context across services

3. **Custom Metrics**:
   - Use the Meter API for custom metrics
   - Add appropriate tags/dimensions
   - Follow naming conventions

4. **Monitoring**:
   - Set up alerts for critical metrics
   - Configure appropriate retention policies
   - Use dashboard variables for filtering

## 8. Next Steps

1. **Alerting**:
   - Set up alerts for critical thresholds
   - Configure notification channels
   - Implement SLOs and SLIs

2. **Custom Business Metrics**:
   - Add domain-specific metrics
   - Create custom dashboards
   - Implement business KPIs

3. **Advanced Correlation**:
   - Enhance correlation between pillars
   - Set up exemplars for metrics→traces
   - Implement custom context propagation
