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
docker-compose up -d prometheus grafana loki tempo rabbitmq rabbitmq-exporter node-exporter mailhog
```

### 2.2. Start Your Application

```bash
cd /Users/WaQas/Desktop/Projects/ConnectFlow/src/Web
dotnet run
```

### 2.3. Generate Sample Traffic

```bash
curl -v https://localhost:5001/api/v1/weatherforecast
```

### 2.4. Access Monitoring Interfaces

- **Grafana Dashboards**: <http://localhost:3000> (admin/admin)
- **Prometheus**: <http://localhost:9090>
- **RabbitMQ Management**: <http://localhost:15672> (guest/guest)
- **MailHog**: <http://localhost:8025>

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
    "LokiUrl": "http://localhost:3100",
    "OtlpEndpoint": "http://localhost:4317",
    "PrometheusUrl": "http://localhost:9090",
    "GrafanaUrl": "http://localhost:3000"
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

## 4. Grafana Dashboards

ConnectFlow includes a comprehensive set of pre-configured Grafana dashboards:

### 4.1 Available Dashboards

- **API Performance** (`api-performance.json`) - HTTP request metrics, response times, error rates
- **Health Dashboard** (`health-dashboard.json`) - System health status, dependency monitoring
- **Payment Dashboard** (`payment-dashboard.json`) - Stripe payment processing, subscription metrics
- **RabbitMQ Dashboard** (`rabbitmq-dashboard.json`) - Message queue monitoring, throughput analysis
- **Quartz Jobs** (`quartz-jobs.json`) - Background job execution, success/failure rates
- **Email Dashboard** (`email-dashboard.json`) - Email sending metrics, template usage
- **Rate Limits** (`rate-limits.json`) - API rate limiting monitoring
- **.NET Runtime** (`dotnet-runtime.json`) - Application runtime metrics, GC performance

### 4.2 Dashboard Access

All dashboards are automatically provisioned and available at:
- **Base URL**: <http://localhost:3000>
- **Login**: admin / admin
- **Direct Dashboard Access**: <http://localhost:3000/dashboards>

### 4.3 Key Dashboard Features

Each dashboard includes:
- **Time Range Selector**: Standard Grafana time controls
- **Variable Filters**: Dynamic filtering by tenant, user, job type, etc.
- **Alert Integration**: Pre-configured alerts for critical metrics
- **Correlation Links**: Easy navigation between related dashboards

## 5. Metrics Collection

### 5.1 Application Metrics

ConnectFlow collects comprehensive application metrics including:

- **HTTP Requests**: Request count, duration, status codes by endpoint
- **Authentication**: Login attempts, token refreshes, authorization failures
- **Payment Processing**: Transaction success/failure rates, payment methods
- **Background Jobs**: Job execution count, duration, success/failure rates
- **Message Processing**: RabbitMQ message throughput, retry rates, dead letter counts
- **Database Operations**: Query execution times, connection pool usage
- **Cache Performance**: Redis hit/miss rates, operation latencies

### 5.2 Infrastructure Metrics

Infrastructure monitoring includes:

- **System Resources**: CPU, memory, disk usage via node-exporter
- **Database Health**: PostgreSQL connection counts, query performance
- **Message Broker**: RabbitMQ queue depths, connection states
- **Cache Layer**: Redis memory usage, command statistics

## 6. Distributed Tracing

### 6.1 Trace Correlation

ConnectFlow implements comprehensive trace correlation:

- **Request Tracing**: End-to-end HTTP request tracking
- **Background Job Tracing**: Job execution spans with context propagation
- **Database Query Tracing**: SQL operation tracing with query details
- **External Service Calls**: Stripe API, email service tracing
- **Message Processing**: RabbitMQ message handling spans

### 6.2 Trace Access

- **Grafana Explore**: <http://localhost:3000/explore?left={"datasource":"tempo"}>
- **Direct Tempo**: <http://localhost:3200>

### 6.3 Common Trace Queries

```
# Find traces by operation name
{operation="HTTP GET /api/v1/subscriptions"}

# Find traces by service
{service.name="ConnectFlow.Web"}

# Find traces with errors
{status=error}

# Find traces by correlation ID
{correlation.id="abc123def456"}
```

## 7. Log Exploration

### 7.1 Log Access and Queries

- **Access**: <http://localhost:3000/explore?left={"datasource":"loki"}>
- **Common Queries**:
  - All logs: `{app="connectflow"}`
  - Error logs: `{app="connectflow"} |= "error" | json`
  - Payment logs: `{app="connectflow"} |= "payment" | json`
  - Stripe API logs: `{app="connectflow"} |= "Stripe" | json`
  - Webhook logs: `{app="connectflow"} |= "webhook" | json`
  - Subscription logs: `{app="connectflow"} |= "subscription" | json`
  - Background job logs: `{app="connectflow"} |= "job" | json`
  - RabbitMQ logs: `{app="connectflow"} |= "RabbitMQ" | json`
  - Message processing logs: `{app="connectflow"} |= "message" |= "processed" | json`
  - Correlation ID tracking: `{app="connectflow"} |= "correlation_id" |= "abc123" | json`

### 7.2 Log Correlation Features

- **Trace Correlation**: Automatic trace_id and span_id inclusion
- **User Context**: Tenant and user information in structured format
- **Request Correlation**: Correlation ID linking across all components
- **Structured Fields**: JSON-formatted logs with searchable fields

## 8. Alerting (Future Enhancement)

ConnectFlow is designed to support alerting through:

- **Prometheus AlertManager**: Metric-based alerting rules
- **Grafana Alerts**: Dashboard-based alert configuration
- **Health Check Integration**: Automatic alerts on health check failures

### 8.1 Recommended Alert Rules

```yaml
# High error rate
- alert: HighErrorRate
  expr: rate(http_requests_total{status=~"5.."}[5m]) > 0.1
  for: 5m
  labels:
    severity: warning
  annotations:
    summary: High error rate detected

# Payment processing failures
- alert: PaymentFailures
  expr: increase(stripe_payment_failed_total[5m]) > 5
  for: 2m
  labels:
    severity: critical
  annotations:
    summary: Multiple payment failures detected
```

## 9. Development and Debugging

### 9.1 Local Development Setup

For local development with full observability:

1. **Start observability stack**: `docker-compose up -d prometheus grafana loki tempo`
2. **Run application with monitoring**: `dotnet run` (monitoring is enabled by default in Development)
3. **Generate sample data**: Use the `/api/v1/weatherforecast` endpoint

### 9.2 Correlation ID Debugging

Every request generates a correlation ID that flows through:
- HTTP request headers
- Log entries (as structured field)
- Distributed traces (as tags)
- Background jobs (in job data)
- RabbitMQ messages (in message headers)

### 9.3 Performance Analysis

Use the observability stack to analyze:
- **Response Time Percentiles**: P50, P95, P99 response times by endpoint
- **Database Query Performance**: Slow query identification
- **Memory Usage Patterns**: GC pressure and allocation rates
- **Background Job Performance**: Job execution times and failure patterns

## 10. Production Considerations

### 10.1 Resource Requirements

- **Prometheus**: 2GB RAM, 50GB disk for 30-day retention
- **Loki**: 1GB RAM, 100GB disk for log storage
- **Tempo**: 1GB RAM, 20GB disk for trace storage
- **Grafana**: 512MB RAM, minimal disk usage

### 10.2 Security Considerations

- **Authentication**: Grafana admin password should be changed
- **Network Security**: Observability endpoints should be firewalled
- **Data Retention**: Configure appropriate retention policies
- **Access Control**: Implement RBAC for dashboard access

### 10.3 High Availability

For production deployments:
- **Prometheus**: Use federation or remote storage
- **Loki**: Deploy with object storage backend
- **Tempo**: Configure distributed storage
- **Grafana**: Use external database for persistence

## 11. Troubleshooting

### 11.1 Common Issues

**Metrics Not Appearing**:
- Check Prometheus targets: <http://localhost:9090/targets>
- Verify application metrics endpoint: <https://localhost:5001/metrics>
- Check Grafana data source configuration

**Logs Not Visible**:
- Verify Loki configuration in `appsettings.json`
- Check Loki ingestion: <http://localhost:3100/ready>
- Validate log format and labels

**Traces Missing**:
- Confirm OTLP endpoint configuration
- Check Tempo readiness: <http://localhost:3200/ready>
- Verify trace sampling configuration

### 11.2 Health Check Endpoints

- **Application Health**: <https://localhost:5001/health>
- **Detailed Health**: <https://localhost:5001/health/ready>
- **Liveness Probe**: <https://localhost:5001/health/live>

### 11.3 Configuration Validation

Check your `appsettings.json` monitoring configuration:

```json
{
  "Monitoring": {
    "UseLoki": true,
    "LokiUrl": "http://localhost:3100",
    "OtlpEndpoint": "http://localhost:4317",
    "PrometheusUrl": "http://localhost:9090",
    "GrafanaUrl": "http://localhost:3000"
  }
}
```

This comprehensive observability setup provides full visibility into ConnectFlow's operation, enabling effective monitoring, debugging, and performance optimization.
