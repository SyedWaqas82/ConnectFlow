# ConnectFlow Unified Observability

This document provides a comprehensive guide to the observability stack in ConnectFlow.

## 1. Observability Stack Overview

ConnectFlow implements a modern unified observability stack based on the MELT (Metrics, Events, Logs, Traces) paradigm:

- **Metrics**: Prometheus collects and stores time-series metrics for real-time monitoring and alerting
- **Logs**: Loki aggregates structured logs with trace correlation for unified debugging
- **Traces**: Tempo provides di### 7.1 Log Exploration

- **Access**: [http://localhost:3000/explore?left={"datasource":"loki"}](http://localhost:3000/explore?left={"datasource":"loki"})
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
  - Correlation ID tracking: `{app="connectflow"} |= "correlation_id" |= "abc123" | json`d tracing functionality with end-to-end request tracking
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

### 3.5 Payment & Stripe Monitoring

ConnectFlow implements comprehensive payment and Stripe integration monitoring for robust financial observability:

- **Payment Processing Tracking**: Real-time monitoring of payment success/failure rates
- **Stripe API Performance**: Detailed metrics for Stripe API call performance and reliability
- **Webhook Processing**: Comprehensive tracking of Stripe webhook delivery and processing
- **Revenue Metrics**: Financial tracking with currency and plan breakdown
- **Subscription Lifecycle**: Complete monitoring of subscription events and state changes
- **Retry Logic Monitoring**: Tracking of payment retry attempts and grace period management

#### 3.5.1 Payment Metrics Collection

The `PaymentMetrics.cs` class collects and exposes the following payment-related metrics:

**Payment Processing Metrics:**
- **`payment.successful`**: Counter tracking successful payment transactions with amount and currency tags
- **`payment.failed`**: Counter tracking failed payments with failure reason and currency tags
- **`payment.revenue`**: Histogram tracking payment amounts for revenue analysis
- **`payment.processing_time`**: Histogram tracking payment processing duration

**Stripe API Metrics:**
- **`stripe.api.calls`**: Counter tracking Stripe API calls by operation and status
- **`stripe.api.duration`**: Histogram tracking Stripe API call response times
- **`stripe.api.errors`**: Counter tracking Stripe API errors by operation and error type
- **`stripe.api.rate_limit`**: Gauge tracking Stripe API rate limit usage

**Webhook Metrics:**
- **`stripe.webhook.received`**: Counter tracking received webhooks by event type
- **`stripe.webhook.processed`**: Counter tracking successfully processed webhooks with processing time
- **`stripe.webhook.failed`**: Counter tracking webhook processing failures by error type and event type
- **`stripe.webhook.retry`**: Counter tracking webhook retry attempts

**Subscription Metrics:**
- **`subscription.created`**: Counter tracking new subscriptions by plan type and amount
- **`subscription.canceled`**: Counter tracking subscription cancellations by type and plan
- **`subscription.updated`**: Counter tracking subscription changes (upgrades/downgrades) by plan transition
- **`subscription.revenue`**: Gauge tracking monthly recurring revenue (MRR) by plan type

#### 3.5.2 Payment Event Tracking

The payment system tracks detailed event flows through the following components:

**Domain Events:**
- `PaymentStatusEvent`: Tracks payment success/failure with retry counting
- `SubscriptionStatusEvent`: Tracks subscription lifecycle changes
- `StripeWebhookEvent`: Tracks webhook processing and validation

**Infrastructure Events:**
- Stripe API call logging with correlation IDs
- Payment retry logic with exponential backoff tracking
- Grace period management and expiration monitoring

#### 3.5.3 Payment Dashboard Panels

The Payment & Stripe dashboard includes specialized panels for comprehensive financial monitoring:

**Revenue Tracking:**
- Monthly Recurring Revenue (MRR) trends
- Payment volume by currency and plan type
- Revenue per customer analysis
- Churn and growth rate calculation

**Payment Processing:**
- Payment success rate (target: >99.5%)
- Payment failure analysis by reason
- Processing time percentiles (p50/p90/p99)
- Retry success rate and patterns

**Stripe Integration Health:**
- API response time monitoring
- Error rate tracking by endpoint
- Rate limit utilization monitoring
- Webhook delivery success rates

**Subscription Analytics:**
- New subscription conversion rates
- Plan upgrade/downgrade patterns
- Cancellation reasons and timing
- Grace period utilization metrics

#### 3.5.4 Payment Alerting

Critical payment alerts configured in the system:

```promql
# High payment failure rate alert (>5% failures in 10 minutes)
(
  rate(payment_failed_total[10m]) / 
  (rate(payment_successful_total[10m]) + rate(payment_failed_total[10m]))
) > 0.05

# Stripe API high error rate (>2% errors in 5 minutes)
(
  rate(stripe_api_errors_total[5m]) / 
  rate(stripe_api_calls_total[5m])
) > 0.02

# Webhook processing failure spike (>10 failures in 5 minutes)
rate(stripe_webhook_failed_total[5m]) > 10

# Revenue drop alert (>20% decrease in hourly revenue)
(
  rate(payment_revenue_sum[1h]) / 
  rate(payment_revenue_sum[1h] offset 1h)
) < 0.8
```

Example PromQL queries for payment monitoring:

```promql
# Payment success rate over time
sum(rate(payment_successful_total[5m])) / 
(sum(rate(payment_successful_total[5m])) + sum(rate(payment_failed_total[5m])))

# Average payment processing time
avg(stripe_api_duration_seconds{operation="create_payment_intent"})

# Revenue by plan type
sum by (plan_type) (rate(payment_revenue_sum[1h]))

# Webhook processing success rate
sum(rate(stripe_webhook_processed_total[5m])) / 
sum(rate(stripe_webhook_received_total[5m]))

# Subscription churn rate
sum(rate(subscription_canceled_total[24h])) / 
sum(rate(subscription_created_total[24h] offset 24h))
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

### 4.3 Payment & Stripe Dashboard

- **Access**: [http://localhost:3000/d/cf-payment-stripe](http://localhost:3000/d/cf-payment-stripe)
- **Purpose**: Monitor payment processing, Stripe API performance, and subscription metrics
- **Key Features**:
  - Payment success/failure rates with trend analysis
  - Revenue tracking with currency breakdown
  - Stripe API call performance and error rates
  - Webhook processing metrics and failure analysis
  - Subscription lifecycle events (created, canceled, upgraded, downgraded)
  - Payment retry patterns and grace period tracking
  - Stripe API quota usage and rate limiting metrics

### 4.4 .NET Runtime Dashboard

- **Access**: [http://localhost:3000/d/cf-dotnet-runtime](http://localhost:3000/d/cf-dotnet-runtime)
- **Purpose**: Monitor application resource usage and performance
- **Key Features**:
  - Memory consumption trends and garbage collection metrics
  - CPU utilization patterns across application components
  - Thread pool statistics for concurrency monitoring
  - Exception tracking with type and frequency analysis

### 4.5 Background Jobs Dashboard (Quartz)

- **Access**: [http://localhost:3000/d/cf-quartz-jobs](http://localhost:3000/d/cf-quartz-jobs)
- **Purpose**: Monitor background job execution and performance
- **Key Features**:
  - Job execution success/failure rates by job type
  - Job duration trends and performance analysis
  - Job queue depth and scheduling metrics
  - Misfire tracking and recovery statistics
  - Job correlation ID tracking for distributed tracing

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

### 5.3 Investigating Payment Processing Issues

1. Start with the **Payment & Stripe Dashboard** to check:
   - Are payment success rates below normal thresholds (target: >99.5%)?
   - Are there spikes in payment failures or specific failure reasons?
   - Is Stripe API performance degraded (high response times or error rates)?
   - Are webhooks being processed successfully?

2. Check for systematic issues:
   - Review Stripe API error patterns by operation type
   - Examine webhook delivery failures and retry patterns
   - Monitor subscription lifecycle events for anomalies
   - Check revenue trends for unexpected drops

3. For detailed investigation:
   - Search logs for specific payment correlation IDs
   - Trace payment flows through the system using distributed tracing
   - Check Stripe Dashboard for additional context and raw webhook data
   - Verify payment retry logic and grace period handling

4. Common solutions:
   - For API errors: Check Stripe service status and API key validity
   - For webhook failures: Verify webhook endpoint accessibility and signature validation
   - For payment failures: Review customer payment methods and billing information
   - For processing delays: Check background job execution and RabbitMQ message processing

### 5.4 Monitoring Background Job Performance

1. Use the **Background Jobs Dashboard** to monitor:
   - Are jobs executing successfully within expected timeframes?
   - Are there job execution backlogs or high failure rates?
   - Are job durations increasing over time indicating performance issues?
   - Are there frequent job misfires or scheduling problems?

2. For job-specific investigation:
   - Filter by specific job types to identify problematic jobs
   - Review job correlation IDs for end-to-end tracing
   - Check job execution patterns and timing consistency
   - Monitor job queue depth and processing capacity

3. Troubleshooting steps:
   - Check application logs for job-specific error details
   - Verify background job infrastructure health (Quartz scheduler)
   - Review job dependencies and resource usage
   - Examine job configuration and scheduling parameters

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

## 6. Cross-System Correlation

ConnectFlow's unified observability stack enables powerful cross-system correlation for comprehensive problem diagnosis:

### 6.1 Payment-to-Infrastructure Correlation

- **Scenario**: Payment processing slowdowns
- **Investigation Flow**:
  1. Start with payment metrics showing increased processing times
  2. Correlate with API performance metrics to identify bottlenecks
  3. Check background job execution for payment-related jobs
  4. Examine RabbitMQ metrics for webhook processing backlogs
  5. Review .NET runtime metrics for resource constraints

### 6.2 Subscription-to-Messaging Correlation

- **Scenario**: Subscription lifecycle event processing delays
- **Investigation Flow**:
  1. Monitor subscription creation/cancellation metrics
  2. Correlate with RabbitMQ message processing rates
  3. Check background job execution for subscription-related jobs
  4. Examine webhook delivery success rates from Stripe
  5. Review application logs for event handler processing times

### 6.3 End-to-End Request Tracing

- **Correlation ID Flow**: Payment Request → API Processing → Background Job → Webhook Processing → Email Notification
- **Trace Components**:
  - HTTP request with correlation ID
  - Payment processing span
  - Background job execution span
  - RabbitMQ message publishing span
  - Email delivery span

## 7. Data Exploration Tools

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
  - `payment.successful`: Successful payment transactions
  - `payment.failed`: Failed payment transactions
  - `payment.revenue`: Payment revenue tracking
  - `stripe.api.calls`: Stripe API call metrics
  - `stripe.api.duration`: Stripe API response times
  - `stripe.webhook.received`: Received webhook events
  - `stripe.webhook.processed`: Successfully processed webhooks
  - `subscription.created`: New subscription metrics
  - `subscription.canceled`: Subscription cancellation metrics
  - `job_executions_total`: Background job execution counts
  - `job_duration_seconds`: Background job execution times
  - `job_errors_total`: Background job error counts

## 8. Best Practices

1. **Structured Logging**:
   - Use semantic logging with named properties
   - Include contextual information (correlation IDs, tenant IDs, user IDs)
   - Use appropriate log levels for different scenarios
   - Include payment-specific context (amount, currency, payment method)

2. **Distributed Tracing**:
   - Create spans for important operations (payments, subscriptions, jobs)
   - Add relevant tags to spans (tenant_id, correlation_id, payment_id)
   - Propagate trace context across services and background jobs
   - Use activity sources for custom instrumentation

3. **Custom Metrics**:
   - Use the Meter API for custom business metrics
   - Add appropriate tags/dimensions for filtering and grouping
   - Follow naming conventions (payment.*, subscription.*, job.*)
   - Track both technical and business metrics

4. **Payment Monitoring**:
   - Monitor payment success rates with tight SLAs (>99.5%)
   - Track revenue metrics with proper currency handling
   - Set up alerts for critical payment thresholds
   - Monitor Stripe API performance and rate limits
   - Track webhook delivery and processing success

5. **Dashboard Design**:
   - Use dashboard variables for filtering (tenant, plan, time range)
   - Implement proper time-based aggregations
   - Set appropriate alert thresholds with context
   - Configure retention policies based on data importance

6. **Correlation Strategy**:
   - Use consistent correlation IDs across all components
   - Implement proper context propagation in async operations
   - Link metrics, logs, and traces through common identifiers
   - Design for cross-system troubleshooting workflows

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
