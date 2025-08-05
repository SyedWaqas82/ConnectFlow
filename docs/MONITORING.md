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
docker-compose up -d prometheus grafana loki tempo rabbitmq rabbitmq-exporter
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

### 3.3 Health Monitoring

ConnectFlow provides production-grade health monitoring through the `HealthChecksConfiguration.cs` implementation:

- **Granular health probes**: Different checks for different operational aspects (liveness vs. readiness)
- **Rich visualization**: Interactive health UI dashboard with historical data
- **Comprehensive coverage**: Monitoring of all system dependencies (database, cache, background jobs)
- **Kubernetes-ready**: Health checks designed for containerized deployments with proper probes
- **Prometheus integration**: Health metrics exposed for advanced visualization and alerting in Grafana

### 3.4 RabbitMQ Monitoring

ConnectFlow implements comprehensive RabbitMQ monitoring for robust messaging observability:

- **Connection Health**: Real-time monitoring of RabbitMQ connection status
- **Queue Metrics**: Detailed metrics for message counts, consumption rates, and processing status
- **Custom Telemetry**: Specialized counters for tracking message flow through the system
- **Integrated Health Checks**: Automatic verification of RabbitMQ availability and connectivity
- **Grafana Dashboards**: Pre-configured visualization of all RabbitMQ metrics

#### 3.4.1 RabbitMQ Metrics Collection

The `RabbitMQMetrics.cs` class collects and exposes the following metrics:

- **`rabbitmq.messages.published`**: Counter tracking total messages published to RabbitMQ
- **`rabbitmq.messages.consumed`**: Counter tracking total messages consumed from RabbitMQ
- **`rabbitmq.messages.failed`**: Counter tracking message processing failures
- **`rabbitmq.messages.retry`**: Counter tracking messages sent to retry queues
- **`rabbitmq.messages.deadletter`**: Counter tracking messages sent to dead-letter queues
- **`rabbitmq.connection.state`**: Gauge indicating connection state (1=connected, 0=disconnected)

Additionally, the RabbitMQ exporter collects native RabbitMQ metrics including:

- **`rabbitmq_connections`**: Number of client connections
- **`rabbitmq_channels`**: Number of channels
- **`rabbitmq_queue_messages`**: Current message count per queue
- **`rabbitmq_queue_messages_ready`**: Messages ready for delivery
- **`rabbitmq_queue_messages_unacknowledged`**: Messages delivered but not yet acknowledged

#### 3.4.2 RabbitMQ Health Check

The `RabbitMQHealthCheck.cs` implements a comprehensive RabbitMQ health verification:

- Connection status verification
- Channel creation capability testing
- Appropriate health status reporting (Healthy/Degraded/Unhealthy)
- Integration with the health dashboard

#### 3.4.3 RabbitMQ Dashboard

ConnectFlow includes a dedicated RabbitMQ health panel in the main health dashboard:

- **Access**: [http://localhost:3000/d/health](http://localhost:3000/d/health)
- **Features**:
  - Connection status monitoring
  - Message throughput visualization
  - Queue depth tracking
  - Failure rate monitoring
  - Channel and connection metrics

#### 3.4.4 RabbitMQ Exporter

The dedicated RabbitMQ exporter collects detailed metrics from the RabbitMQ management API:

- **Container**: `rabbitmq-exporter`
- **Port**: 9419
- **Configuration**:
  - Customized queue filtering with `INCLUDE_QUEUES` parameter
  - Specialized metrics collection for dead-letter and retry queues
  - Connection monitoring with appropriate scrape intervals
  - Integration with Prometheus for metrics storage

Example PromQL queries for RabbitMQ monitoring:

```promql
# Queue depth over time for a specific queue
rate(rabbitmq_queue_messages{queue="my-service-queue"}[5m])

# Message consumption rate
rate(rabbitmq_queue_messages_delivered_total{queue="my-service-queue"}[5m])

# Connection count
rabbitmq_connections

# Channel count 
rabbitmq_channels
```

## 4. Pre-configured Dashboards

ConnectFlow includes specialized dashboards for different monitoring needs:

### 4.1 Health Dashboard

- **Access**: [http://localhost:3000/d/connectflow-health](http://localhost:3000/d/connectflow-health)
- **Purpose**: Monitor overall system health and component status
- **Key Features**:
  - Color-coded health status indicators (green/yellow/red)
  - Historical health trends with timeline view
  - Component-specific health details with drill-down capability
  - Direct links to component-specific metrics and logs
  - RabbitMQ health status panel
  - RabbitMQ connection and channel metrics
  - Queue depth monitoring

### 4.2 API Performance Dashboard

- **Access**: [http://localhost:3000/d/cf-api-performance](http://localhost:3000/d/cf-api-performance)
- **Purpose**: Track API response times, throughput, and error rates
- **Key Features**:
  - Request volume by endpoint with time-based trends
  - Response time percentiles (p50/p90/p99) for detecting slowness
  - Error rate tracking with status code breakdown
  - Filter by endpoint, method, or status code using dashboard variables

### 4.3 .NET Runtime Dashboard

- **Access**: [http://localhost:3000/d/cf-dotnet-runtime](http://localhost:3000/d/cf-dotnet-runtime)
- **Purpose**: Monitor application resource usage and performance
- **Key Features**:
  - Memory consumption trends and garbage collection metrics
  - CPU utilization patterns across application components
  - Thread pool statistics for concurrency monitoring
  - Exception tracking with type and frequency analysis

## 5. Common Monitoring Scenarios

### 5.1 Investigating System Slowness

1. Start with the **API Performance Dashboard** to check:
   - Are response times elevated across all endpoints or just specific ones?
   - Is request volume unusually high?
   - Are there error spikes correlating with slowness?

2. Check the **.NET Runtime Dashboard** for:
   - High memory usage or frequent garbage collections
   - CPU spikes or thread pool exhaustion
   - Exception rate increases

3. For detailed investigation:
   - Use the **Explore** view to search logs during the slow period
   - Sample traces from slow endpoints to identify bottlenecks
   - Check for concurrent background jobs that might be consuming resources

### 5.2 Troubleshooting RabbitMQ Issues

1. Start with the **Health Dashboard** to check:
   - Is the RabbitMQ connection showing as healthy?
   - Are there any recent state changes in the RabbitMQ connection?
   - Are other dependent services also affected?

2. Investigate RabbitMQ metrics:
   - Check for increasing queue depths which may indicate processing backlogs
   - Look for connection drops or channel closures
   - Monitor message publish/consume rates for unusual patterns

3. For detailed diagnostics:
   - Check RabbitMQ logs: `docker-compose logs rabbitmq`
   - Verify connectivity with the health check endpoint: `curl http://localhost:5000/health/ready`
   - Examine the RabbitMQ Management UI: `http://localhost:15672` (guest/guest)

4. Common solutions:
   - For connection issues: Check network connectivity and credentials
   - For queue backlogs: Verify consumer services are running and processing messages
   - For high message failure rates: Check error logs for processing exceptions
   - For performance issues: Consider increasing consumer concurrency or improving message processing efficiency

## 6. Data Exploration Tools

### 6.1 Log Exploration

- **Access**: [http://localhost:3000/explore?left={"datasource":"loki"}](http://localhost:3000/explore?left={"datasource":"loki"})
- **Common Queries**:
  - All logs: `{app="connectflow"}`
  - Error logs: `{app="connectflow"} |= "error" | json`
  - RabbitMQ logs: `{app="connectflow"} |= "RabbitMQ" | json`
  - Message processing logs: `{app="connectflow"} |= "message" |= "processed" | json`

### 6.2 Metrics Exploration

- **Grafana**: [http://localhost:3000/explore?left={"datasource":"prometheus"}](http://localhost:3000/explore?left={"datasource":"prometheus"})
- **Direct Prometheus**: [http://localhost:9090](http://localhost:9090)
- **Key Metrics**:
  - `http_request_duration_seconds`: API response time
  - `http_requests_total`: Request count by endpoint and status
  - `health_status`: Overall system health (0=Unhealthy, 1=Healthy)
  - `health_check_status`: Individual component health (0=Unhealthy, 1=Degraded, 2=Healthy)
  - `rabbitmq_queue_messages`: Current message count per queue
  - `rabbitmq.messages.published`: Total messages published (custom metric)
  - `rabbitmq.messages.consumed`: Total messages consumed (custom metric)
  - `rabbitmq.connection.state`: RabbitMQ connection state (custom metric)

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
