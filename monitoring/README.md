# ConnectFlow Unified Observability

This document provides a comprehensive guide to the observability stack in ConnectFlow.

## 1. Observability Stack Overview

ConnectFlow implements a modern unified observability stack based on the MELT (Metrics, Events, Logs, Traces) paradigm:

- **Metrics**: Prometheus collects and stores time-series metrics for real-time monitoring and alerting
- **Logs**: Loki aggregates structured logs with trace correlation for unified debugging
- **Traces**: Tempo provides distributed tracing functionality with end-to-end request tracking
- **Health Checks**: Comprehensive monitoring of all system components with specialized probes
- **Dashboards**: Grafana visualizes all observability data in customized, role-specific views

### 1.1 Architecture Components

The observability architecture consists of these key components:

```ascii
┌─────────────┐     ┌───────────────┐     ┌─────────────┐
│ Application │────►│ OpenTelemetry │────►│  Exporters  │
│    Code     │     │    SDK        │     │             │
└─────────────┘     └───────────────┘     └──────┬──────┘
       ▲                    ▲                    │
       │                    │                    ▼
┌──────┴──────┐    ┌────────┴─────┐    ┌─────────────────┐
│ Correlation │    │   Serilog    │    │  Storage &      │
│ Middleware  │    │  Structured  │    │  Visualization  │
└─────────────┘    │   Logging    │    │  (Prometheus,   │
                   └──────────────┘    │  Loki, Tempo,   │
                                       │   Grafana)      │
                                       └─────────────────┘
```

### 1.2 Integration Points

- **Correlation ID Generation**: Unique request identifiers flow through all system components
- **Cross-Component Correlation**: Metrics, logs, and traces are linked via common identifiers
- **Middleware Integration**: Automatic propagation of context through HTTP pipeline
- **Dashboard Organization**: Predefined views for different roles and use cases

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

## 3. Observability Components Implementation

### 3.1 OpenTelemetry Implementation

ConnectFlow uses OpenTelemetry as the foundational layer for all observability signals. It provides:

- **Distributed tracing**: Complete request flow visualization with context propagation
- **Automatic instrumentation**: Built-in support for ASP.NET Core, HttpClient, and SQL operations
- **Custom metrics collection**: Application-specific metrics via the Meter API
- **Correlation**: Automatic linking between traces, metrics, and logs through common identifiers

Implementation details:

- The `ObservabilityConfiguration.cs` class configures OpenTelemetry with both trace and metrics providers
- Custom ActivitySource named "ConnectFlow" allows for explicit instrumentation in critical code paths
- Automatic propagation of context through HTTP headers for distributed system tracing
- Tenant and correlation ID enrichment for comprehensive request tracking

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

### 3.2 Structured Logging (Serilog)

ConnectFlow implements comprehensive structured logging using Serilog with the following features:

- **Multiple output sinks**: Simultaneous logging to console, file, and Loki
- **Trace correlation**: Automatic inclusion of `trace_id` and `span_id` via the `TelemetryEnricher`
- **Dynamic configuration**: Environment-based filtering and configurable log levels
- **Rich context**: Exception enrichment and structured data for powerful querying

The `SerilogConfiguration.cs` file integrates logging with other observability systems:

- Configures appropriate log levels for different components (ASP.NET, EF Core, Quartz)
- Enriches logs with machine, environment, process, and thread information
- Creates separate log streams for general logs and error-specific logs
- Conditionally enables Loki integration based on environment and configuration

Log Locations:

- Development logs: `logs/connectflow-dev-.log`
- Error logs: `logs/connectflow-dev-error-.log`
- Grafana Loki: [View in Grafana](http://localhost:3000/explore?orgId=1&left={"datasource":"loki","queries":[{"refId":"A","expr":"{app=\"connectflow\"}"}]})

#### HTTP Request Logging

ConnectFlow automatically logs detailed information for each HTTP request:

- Client IP address (respects X-Forwarded-For headers)
- Client ID from JWT tokens
- Tenant ID from headers or claims
- HTTP method and endpoint path
- Response status code
- Response time in milliseconds
- Correlation ID linking the request to traces and other logs

This enables powerful filtering and analysis in Grafana Loki:

```logql
{app="connectflow"} | HttpMethod="POST" | StatusCode>=400
```

Multi-tenant analysis is also supported:

```logql
{app="connectflow"} | TenantId="customer123" | StatusCode>=500
```

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

### 3.3 Health Monitoring

ConnectFlow provides production-grade health monitoring through the `HealthChecksConfiguration.cs` implementation:

- **Granular health probes**: Different checks for different operational aspects (liveness vs. readiness)
- **Rich visualization**: Interactive health UI dashboard with historical data
- **Comprehensive coverage**: Monitoring of all system dependencies (database, cache, background jobs)
- **Kubernetes-ready**: Health checks designed for containerized deployments with proper probes

The health checking system categorizes checks by tags:

- `live` tags: Verify the application is running and responsive
- `ready` tags: Verify the application can accept and process requests
- Database-specific checks using both EF Core and direct PostgreSQL verification
- Service-specific checks for Quartz job processing

Endpoints:

- Basic health: `/health` - Provides overall health status
- Liveness probe: `/health/live` - For container orchestration to detect if the app is running
- Readiness probe: `/health/ready` - For container orchestration to detect if the app can accept traffic
- Health UI dashboard: `/healthz` - Rich UI for administrators with historical trends

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

### 3.4 Rate Limiting

ConnectFlow implements comprehensive API protection through the `RateLimitingConfiguration.cs` component:

- **Multi-level protection**: Both global (IP-based) and tenant-specific limits
- **Advanced algorithms**: Combination of fixed window and token bucket strategies
- **Prometheus integration**: Rich metrics collection for monitoring and alerting
- **Customizable responses**: Standardized rejection handling with informative responses

The rate limiting implementation tracks detailed metrics:

- `rate_limit_exceeded_total`: Counter tracking rejected requests with endpoint labels
- Request throttling statistics with path-specific details
- Per-tenant usage patterns to detect abuse or capacity issues

Protection features:

- Global IP-based rate limiting for basic DDoS protection
- Tenant-specific API limits for fair resource allocation
- Fixed window algorithm for straightforward request capping
- Token bucket algorithm for burst handling with controlled replenishment

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

### 3.5 Background Jobs (Quartz)

ConnectFlow implements enterprise-grade job scheduling via `QuartzConfiguration.cs`:

- **Production-ready scheduling**: Industrial-strength job execution with cron expressions
- **Persistent storage**: PostgreSQL-backed job store for durability across restarts
- **Automatic recovery**: Recovery from failures with transactional execution
- **Observability integration**: Rich metrics exposed to Prometheus

The implementation includes:

- Schema initialization and management for PostgreSQL storage
- Comprehensive metrics collection with Prometheus integration
- Support for both ad-hoc and scheduled jobs
- Proper error handling and job failure tracking

Key features:

- Scheduled job execution with cron expressions for precise timing
- Persistent job storage in PostgreSQL for durability
- Optional clustered job execution for high availability
- Enhanced metrics collection for comprehensive monitoring:
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
    private readonly ILogger<DataCleanupJob> _logger;
    
    public DataCleanupJob(ILogger<DataCleanupJob> logger)
    {
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            // Use structured logging with job context
            _logger.LogInformation("Starting data cleanup job with fire time {FireTime}", context.FireTimeUtc);
            
            // Job implementation...
            
            _logger.LogInformation("Data cleanup job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing data cleanup job");
            throw; // Re-throw for Quartz to track the failure
        }
    }
}
```

### 3.6 Request Correlation and Distributed Tracing

ConnectFlow implements end-to-end request correlation via `CorrelationMiddleware.cs`:

- **Unique request identification**: Every request gets a correlation ID, either from incoming headers or newly generated
- **Automatic propagation**: Correlation ID flows through all system layers (HTTP, logs, traces, database)
- **Tracing integration**: Activity-based tracking with OpenTelemetry for distributed tracing
- **Context enrichment**: Serilog context automatically enhanced with correlation information

Implementation:

```csharp
// Example request flow with correlation
→ Request comes in (with or without X-Correlation-ID)
→ CorrelationMiddleware assigns or preserves ID
→ ID added to HttpContext.Items["CorrelationId"]
→ Response header X-Correlation-ID set
→ Activity created with correlation tag
→ Serilog LogContext enriched with ID
→ All logs and traces automatically include correlation
→ Response returns to client with correlation ID
```

This correlation system enables powerful observability capabilities:

- Tracking requests across multiple services
- Correlating logs and traces for a single transaction
- Identifying related events in complex processing flows
- Finding all actions performed as part of a specific request

## 4. Integration Architecture

### 4.1 Component Integration

ConnectFlow implements a unified observability architecture where all components work together:

```ascii
┌───────────────────────────────────────────────────────────────┐
│                       ASP.NET Core Application                │
├───────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐        │
│  │ Correlation │    │ Rate        │    │ Exception   │        │
│  │ Middleware  │    │ Limiting    │    │ Handling    │        │
│  └──────┬──────┘    └──────┬──────┘    └──────┬──────┘        │
│         │                  │                  │               │
│         ▼                  ▼                  ▼               │
│  ┌─────────────────────────────────────────────────┐          │
│  │           Application Request Pipeline          │          │
│  └──────────────────────┬──────────────────────────┘          │
│                         │                                     │
│                         ▼                                     │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐        │
│  │ OpenTelemetry│    │ Structured  │    │ Health     │        │
│  │ SDK         │    │ Logging     │    │ Checks     │        │
│  └──────┬──────┘    └──────┬──────┘    └──────┬──────┘        │
│         │                  │                  │               │
└─────────┼──────────────────┼──────────────────┼───────────────┘
          │                  │                  │
          ▼                  ▼                  ▼
┌─────────────────┐  ┌──────────────┐  ┌────────────────┐
│    Tempo        │  │    Loki      │  │   Prometheus   │
│  (Traces)       │  │   (Logs)     │  │   (Metrics)    │
└────────┬────────┘  └───────┬──────┘  └────────┬───────┘
         │                   │                  │
         └───────────────────┼──────────────────┘
                             │
                             ▼
                      ┌──────────────┐
                      │   Grafana    │
                      │ (Dashboards) │
                      └──────────────┘
```

Key integration points:

1. **Correlation Context Propagation**: Unique identifiers flow through all system layers
2. **Centralized Configuration**: All observability settings in a unified structure
3. **Combined Data Views**: Grafana dashboards that merge multiple data sources
4. **Health Status Integration**: Health metrics feed into the overall monitoring system
5. **Background Job Observability**: Quartz jobs exposed via the same monitoring pipeline
6. **Rate Limiting Insights**: API protection metrics integrated with general observability

### 4.2 Main Access Points

- **Grafana**: [http://localhost:3000](http://localhost:3000) (admin/admin)
- **Prometheus**: [http://localhost:9090](http://localhost:9090)
- **Application Metrics**: [http://localhost:5000/metrics](http://localhost:5000/metrics)
- **Health Dashboard**: [http://localhost:5000/healthz](http://localhost:5000/healthz)

### 4.3 Custom Dashboards

ConnectFlow includes a suite of pre-built Grafana dashboards tailored for different user roles and monitoring needs. These dashboards are automatically provisioned through the `/monitoring/grafana/provisioning/dashboards` configuration.

#### API Performance Dashboard

- **URL**: [http://localhost:3000/d/cf-api-performance](http://localhost:3000/d/cf-api-performance)
- **Target audience**: API Operations team, Service Reliability Engineers
- **Update frequency**: Near real-time (15s refresh)
- **Key metrics**:
  - Request rates by endpoint with success/failure breakdown
  - P50/P90/P99 latency percentiles by endpoint
  - Status code distribution over time
  - Error rates by endpoint with trend analysis
  - Authentication flow success rates and duration

#### .NET Runtime Dashboard

- **URL**: [http://localhost:3000/d/cf-dotnet-runtime](http://localhost:3000/d/cf-dotnet-runtime)
- **Target audience**: Platform Engineers, Performance Engineers
- **Update frequency**: 30s refresh
- **Key metrics**:
  - Memory usage trends and GC statistics (collections by generation)
  - CPU utilization by component and request type
  - Thread pool utilization and thread count
  - HTTP request processing time breakdown

#### Quartz Jobs Dashboard

- **URL**: [http://localhost:3000/d/cf-quartz-jobs](http://localhost:3000/d/cf-quartz-jobs)
- **Target audience**: Operations team, Job Administrators
- **Update frequency**: 1m refresh
- **Key metrics**:
  - Job execution success/failure rates with trend analysis
  - Execution duration statistics (p50, p95) by job type
  - Queue depth monitoring with alerting thresholds
  - Job misfire detection and historical trends
  - Error counts by job with categorization
  - Success rate percentage with SLA tracking
  - Scheduler running status and health
  - Time to next job execution for all scheduled jobs

#### Rate Limiting Dashboard

- **URL**: [http://localhost:3000/d/cf-rate-limiting](http://localhost:3000/d/cf-rate-limiting)
- **Target audience**: Security team, API Operations
- **Update frequency**: Near real-time (15s refresh)
- **Key metrics**:
  - Request throttling events by tenant and IP
  - Rejected request counts with endpoint breakdown
  - Limit utilization by tenant with forecasting
  - Token bucket metrics including replenishment visualization

#### Unified System Health Dashboard

- **URL**: [http://localhost:3000/d/cf-system-health](http://localhost:3000/d/cf-system-health)
- **Target audience**: Executive/Management view, Operations team
- **Update frequency**: 1m refresh
- **Key metrics**:
  - Overall system health status from all health checks
  - Critical system components status (database, cache, external APIs)
  - End-to-end latency for key business transactions
  - Error rate trends across all system components
  - Alerting status and recent notifications

### 4.4 Exploration and Debugging

ConnectFlow provides dedicated exploration views for ad-hoc analysis and debugging:

- **Metrics Explorer**: [http://localhost:3000/explore?orgId=1&left={"datasource":"prometheus"}](http://localhost:3000/explore?orgId=1&left={"datasource":"prometheus"})
  - For custom queries against all collected metrics
  - Supports PromQL for advanced time-series analysis
  - Example: `rate(http_request_duration_seconds_count{status_code=~"5.."}[5m])`

- **Logs Explorer**: [http://localhost:3000/explore?orgId=1&left={"datasource":"loki"}](http://localhost:3000/explore?orgId=1&left={"datasource":"loki"})
  - Advanced log filtering by labels and content
  - Search by correlation ID, tenant, or error type
  - Example query: `{app="connectflow"} |= "error" | json | tenant_id=~"customer.*" | line_format "{{.message}}"`

- **Traces Explorer**: [http://localhost:3000/explore?orgId=1&left={"datasource":"tempo"}](http://localhost:3000/explore?orgId=1&left={"datasource":"tempo"})
  - Find traces by service, operation, or duration
  - Analyze request flow across system components
  - Correlate with logs using trace ID
  - Visual representation of full request processing

- **Service Graph**: [http://localhost:3000/explore?orgId=1&left={"datasource":"tempo","queryType":"serviceMap"}](http://localhost:3000/explore?orgId=1&left={"datasource":"tempo","queryType":"serviceMap"})
  - Visual representation of service dependencies
  - Traffic volume between components
  - Error rate indicators
  - Request flow visualization

## 5. Unified Data Correlation and Best Practices

### 5.1 Trace Context and Correlation

All observability data in ConnectFlow is linked through common identifiers:

- **`trace_id`**: Unique identifier generated by OpenTelemetry for a request flow
- **`span_id`**: Identifier for a specific operation within a trace
- **`X-Correlation-Id`**: Custom correlation identifier managed by the CorrelationMiddleware
- **`tenant.id`**: Multi-tenancy context for organization-specific filtering

The correlation system ensures that a single request can be tracked across:

- HTTP request logs from middleware
- Application logs from business logic
- Database operation spans
- Cache interaction metrics
- Background job executions (when triggered by a request)
- Downstream service calls

### 5.2 Observability Best Practices

ConnectFlow implements these observability best practices:

1. **Consistent Context Propagation**: Every request maintains its context across all layers
2. **Structured Data**: All logs use structured format with consistent property names
3. **Semantic Logging**: Log messages contain meaningful business context, not just technical details
4. **Error Enrichment**: Exceptions are automatically enriched with additional context
5. **Health Segregation**: Health checks are categorized by purpose (liveness vs. readiness)
6. **Custom Metrics**: Business-relevant metrics supplementing standard system metrics
7. **Dashboard Organization**: Role-specific views with appropriate detail levels
8. **Alert Integration**: Monitoring thresholds tied to SLOs and business impact

### 5.3 Development Guidelines

When extending ConnectFlow, follow these observability guidelines:

```csharp
// 1. Use structured logging with semantic properties
_logger.LogInformation("Processing order {OrderId} for {CustomerId}", order.Id, customer.Id);

// 2. Add meaningful context to logs for complex operations
using(LogContext.PushProperty("TransactionId", transactionId))
{
    _logger.LogInformation("Starting complex transaction");
    // Operation code
}

// 3. Create custom metrics for business-significant events
_orderProcessedCounter.Add(1, 
    new KeyValuePair<string, object?>("customer_type", customer.Type),
    new KeyValuePair<string, object?>("order_size", order.Items.Count));

// 4. Create spans for significant operations
using var activity = ConnectFlowTracing.ActivitySource.StartActivity("ProcessOrder");
activity?.SetTag("order.id", order.Id);
activity?.SetTag("customer.id", customer.Id);
```

## 6. Conclusion and Future Roadmap

### 6.1 Current Observability Maturity

The ConnectFlow observability implementation represents a mature, production-ready monitoring solution with:

- Complete **end-to-end visibility** across all system components
- **Unified correlation** between metrics, logs, and traces
- **Role-specific dashboards** for different stakeholders
- **Proactive monitoring** capabilities through alerting and anomaly detection
- **Business-level insights** alongside technical operational metrics

### 6.2 Future Enhancements

Planned improvements to the observability platform include:

1. **Advanced Alerting**: Implementation of ML-based anomaly detection for proactive issue identification
2. **SLO Tracking**: Service Level Objective monitoring with error budgets
3. **Business Metrics**: Additional dashboard views for product and business stakeholders
4. **Automatic RCA**: Root Cause Analysis automation for common failure patterns
5. **Cross-Environment Correlation**: Enhanced correlation across development, staging, and production

### 6.3 Maintenance and Support

For assistance with the observability platform:

- **Dashboard Issues**: Contact the Platform Engineering team
- **Custom Metrics**: See development guidelines in section 5.3
- **Alert Configuration**: Review alert documentation in Grafana
- **Observability Upgrades**: Check the quarterly platform upgrade schedule

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
