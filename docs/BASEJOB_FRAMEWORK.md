# ConnectFlow BaseJob Framework Documentation

## Overview

The ConnectFlow BaseJob framework provides a comprehensive foundation for enterprise-grade background job processing with Quartz.NET. All jobs in the system inherit from `BaseJob`, which provides automatic correlation tracking, comprehensive metrics collection, structured logging, distributed tracing, and context management.

## Architecture Features

### 🔄 Automatic Context Management
- Tenant and user context initialization
- Secure context isolation between jobs
- Helper methods for accessing current context

### 📊 Comprehensive Metrics
- `job_executions_total` - Success/error counters by job type
- `job_duration_seconds` - Execution time histogram  
- `job_errors_total` - Error counters by error type and job

### 🔍 Correlation Tracking
- Automatic correlation ID generation
- Correlation ID propagation through logs and traces
- External service integration support

### 📝 Structured Logging
- Automatic LogContext properties injection
- Job data parameters in logs
- Execution timing and status

### 🌐 Distributed Tracing
- OpenTelemetry integration
- Activity/span creation with tags
- Job data and timing information

### ⚡ Enhanced Scheduling
- Simple extension methods for job scheduling
- Built-in correlation ID support
- Flexible context and data passing

## BaseJob Implementation

### Core Features

```csharp
public abstract class BaseJob : IJob
{
    protected readonly ILogger Logger;
    protected readonly IContextManager ContextManager;
    
    // Automatic metrics collection
    // Correlation ID management
    // LogContext property injection
    // Distributed tracing
    // Context initialization
}
```

### What You Get Automatically

**Structured Logging:**
```
[13:32:06 INF] Starting job DataCleanupJob (ID: instance-123) with correlation ID abc123def456
[13:32:08 INF] Job DataCleanupJob (ID: instance-123) completed successfully in 2.34s
```

**Metrics (Prometheus-compatible):**
```
# Execution counters
job_executions_total{job_name="DataCleanupJob",status="success"} 42
job_executions_total{job_name="DataCleanupJob",status="error"} 2

# Duration histogram
job_duration_seconds{job_name="DataCleanupJob",quantile="0.5"} 1.23
job_duration_seconds{job_name="DataCleanupJob",quantile="0.95"} 3.45

# Error counters by type
job_errors_total{job_name="DataCleanupJob",error_type="SqlException"} 1
```

**LogContext Properties (automatic):**
- `CorrelationId` - Unique correlation identifier
- `JobName` - Class name of the job
- `JobId` - Quartz execution instance ID  
- `FireTime` - When the job was triggered
- `NextFireTime` - Next scheduled execution
- `Job.{ParameterName}` - All job data parameters

## Job Development Guide

### Creating a New Job

```csharp
[DisallowConcurrentExecution]
public class MyBusinessJob : BaseJob
{
    private readonly IMyService _myService;

    public MyBusinessJob(
        ILogger<MyBusinessJob> logger,
        IContextManager contextManager,
        IMyService myService)
        : base(logger, contextManager)
    {
        _myService = myService;
    }

    protected override async Task ExecuteJobAsync(IJobExecutionContext context)
    {
        // Context is automatically initialized
        var tenantId = GetCurrentTenantId();
        var userId = GetCurrentApplicationUserId();
        var correlationId = GetCurrentCorrelationId(context);

        // Get job parameters
        var batchSize = context.MergedJobDataMap.GetInt("BatchSize");
        
        Logger.LogInformation("Processing {BatchSize} items for tenant {TenantId}", 
            batchSize, tenantId);

        // Your business logic here
        await _myService.ProcessDataAsync(batchSize, correlationId, context.CancellationToken);

        Logger.LogInformation("Processing completed successfully");
    }
}
```

### Migrating Existing Jobs

**4-Step Migration Process:**

1. **Change inheritance**: `IJob` → `BaseJob`
2. **Update constructor**: Add `IContextManager` parameter, call base constructor
3. **Change method**: `Execute` → `ExecuteJobAsync`
4. **Update logger**: `_logger` → `Logger`

**Before Migration:**
```csharp
public class OldJob : IJob
{
    private readonly ILogger<OldJob> _logger;

    public OldJob(ILogger<OldJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting job");
        // Manual correlation tracking
        // Manual metrics collection
        // Manual context management
    }
}
```

**After Migration:**
```csharp
public class OldJob : BaseJob
{
    public OldJob(ILogger<OldJob> logger, IContextManager contextManager)
        : base(logger, contextManager)
    {
    }

    protected override async Task ExecuteJobAsync(IJobExecutionContext context)
    {
        Logger.LogInformation("Starting job");
        // Automatic correlation tracking ✅
        // Automatic metrics collection ✅  
        // Automatic context management ✅
    }
}
```

## Job Scheduling

### Enhanced QuartzExtensions

```csharp
// Schedule with specific tenant and user context
await scheduler.ScheduleJobWithContextAsync<MyBusinessJob>(
    tenantId: 123,
    applicationUserId: 456,
    correlationId: "user-initiated-process",
    additionalData: new JobDataMap { ["BatchSize"] = 100 }
);

// Schedule tenant job (uses default admin)
await scheduler.ScheduleTenantJobAsync<MyBusinessJob>(
    tenantId: 123,
    correlationId: "tenant-maintenance"
);

// Schedule system job (no tenant context)
await scheduler.ScheduleSystemJobAsync<MyBusinessJob>(
    correlationId: "system-cleanup"
);

// Schedule with cron expression
await scheduler.ScheduleJobWithContextAsync<MyBusinessJob>(
    tenantId: 123,
    cronExpression: "0 0 2 * * ?", // 2 AM daily
    correlationId: "daily-reports"
);

// Schedule with delay
await scheduler.ScheduleJobWithContextAsync<MyBusinessJob>(
    tenantId: 123,
    delay: TimeSpan.FromMinutes(30),
    correlationId: "delayed-processing"
);
```

### Configuration-Based Scheduling

Jobs are now automatically registered via the enhanced JobSchedulingService in `QuartzConfiguration.cs`:

```csharp
// Jobs are scheduled using enhanced BaseJob extensions
public class JobSchedulingService : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // DataCleanupJob: runs daily at 2:00 AM (system job)
        await ScheduleJobFromConfig<DataCleanupJob>(
            defaultCron: "0 0 2 * * ?",
            description: "Daily data cleanup job"
        );

        // SubscriptionGracePeriodJob: runs every hour (system job)  
        await ScheduleJobFromConfig<SubscriptionGracePeriodJob>(
            defaultCron: "0 0 * * * ?",
            description: "Hourly subscription grace period check"
        );

        // ReportGenerationJob with job data and correlation tracking
        await ScheduleJobFromConfig<ReportGenerationJob>(
            defaultCron: "0 0 8 ? * MON",
            description: "Weekly report generation job",
            suffix: "Weekly",
            additionalData: new JobDataMap { ["ReportType"] = "Weekly" }
        );
    }
}
```

**Benefits of Enhanced Scheduling:**
- ✅ Automatic correlation ID generation for each scheduled job
- ✅ Enhanced logging with job scheduling details
- ✅ Configuration-driven job parameters
- ✅ Built-in error handling and retry capabilities
- ✅ Consistent context management across all jobs

## Production Jobs in ConnectFlow

### DataCleanupJob
- **Schedule**: Daily at 2:00 AM
- **Purpose**: Cleanup expired data, audit logs, temporary files
- **Features**: Automatic correlation tracking, metrics collection

### SubscriptionGracePeriodJob  
- **Schedule**: Hourly
- **Purpose**: Process expired subscription grace periods and auto-downgrades
- **Features**: Full context management, complex business logic, error handling

### ReportGenerationJob
- **Schedule**: Weekly (Monday 8 AM) and Monthly (1st at 6 AM)
- **Purpose**: Generate system and business reports
- **Features**: Job data parameters (ReportType), correlation tracking

### TestMetricsJob
- **Schedule**: Every minute
- **Purpose**: Generate test metrics and simulate various execution scenarios
- **Features**: Random error simulation, varying execution times

### LongRunningJob
- **Schedule**: Every 3 seconds
- **Purpose**: Simulate long-running processes and misfire scenarios
- **Features**: Cancellation support, misfire handling

## Monitoring & Observability

### Grafana Dashboard
- **URL**: http://localhost:3000/d/quartz/quartz-jobs-dashboard?orgId=1
- **Metrics**: Job execution counts, durations, error rates
- **Alerts**: Failed job notifications, long-running job warnings

### Log Aggregation
All jobs automatically include structured logging with:
- Correlation IDs for request tracing
- Job parameters and context
- Execution timing and status
- Error details with stack traces

### Distributed Tracing
- OpenTelemetry integration
- Automatic span creation with job metadata
- Correlation ID propagation to external services

## Configuration

### appsettings.json
```json
{
  "QuartzJobs": {
    "DataCleanupJob": {
      "CronExpression": "0 0 2 * * ?",
      "Description": "Daily data cleanup",
      "Enabled": true
    },
    "ReportGenerationJob.Weekly": {
      "CronExpression": "0 0 8 ? * MON",
      "Description": "Weekly reports",
      "Enabled": true
    }
  }
}
```

### Database Persistence
- PostgreSQL-based job storage
- Clustering support for high availability
- Automatic schema management
- Job recovery and persistence

## Advanced Usage

### External Service Integration
```csharp
protected override async Task ExecuteJobAsync(IJobExecutionContext context)
{
    var correlationId = GetCurrentCorrelationId(context);
    
    // Pass correlation ID to external services
    await _externalApiService.ProcessAsync(data, correlationId);
    await _emailService.SendNotificationAsync(email, correlationId);
    
    // Custom business metrics
    _customMetrics.RecordBatchSize(processedCount);
}
```

### Error Handling & Retry
```csharp
protected override async Task ExecuteJobAsync(IJobExecutionContext context)
{
    try
    {
        await ProcessDataAsync();
    }
    catch (TransientException ex)
    {
        Logger.LogWarning(ex, "Transient error occurred, job will be retried");
        throw; // Let Quartz handle retry
    }
    catch (FatalException ex)
    {
        Logger.LogError(ex, "Fatal error occurred, marking job as failed");
        // Don't rethrow for fatal errors that shouldn't be retried
    }
}
```

### Context-Aware Processing
```csharp
protected override async Task ExecuteJobAsync(IJobExecutionContext context)
{
    var tenantId = GetCurrentTenantId();
    var userId = GetCurrentApplicationUserId();
    
    if (tenantId.HasValue)
    {
        // Process tenant-specific data
        await ProcessTenantDataAsync(tenantId.Value);
    }
    else
    {
        // Process system-wide data
        await ProcessSystemDataAsync();
    }
}
```

## Best Practices

### ✅ Do
- Always inherit from `BaseJob`
- Use correlation IDs for external service calls
- Log important business events
- Handle cancellation tokens properly
- Use `[DisallowConcurrentExecution]` for data integrity
- Pass tenant/user context when scheduling
- Use structured logging with parameters

### ❌ Don't
- Implement `IJob` directly
- Ignore cancellation tokens
- Log sensitive data
- Use blocking synchronous calls
- Forget error handling
- Create jobs without correlation tracking
- Use static loggers

## Migration Checklist

- [ ] ✅ All jobs inherit from `BaseJob`
- [ ] ✅ Constructors updated with `IContextManager`
- [ ] ✅ Methods changed from `Execute` to `ExecuteJobAsync`
- [ ] ✅ Logger references updated to use `Logger` property
- [ ] ✅ QuartzConfiguration updated to use enhanced scheduling
- [ ] ✅ Job data parameters properly configured
- [ ] ✅ Correlation ID tracking implemented
- [ ] ✅ Metrics collection verified
- [ ] ✅ Structured logging confirmed
- [ ] ✅ Build and deployment successful

## Summary

The ConnectFlow BaseJob framework provides enterprise-grade background job processing with minimal setup. All jobs automatically get:

🔄 **Context Management** - Automatic tenant/user context initialization  
📊 **Metrics Collection** - Comprehensive execution and error metrics  
🔍 **Correlation Tracking** - End-to-end request tracing  
📝 **Structured Logging** - Rich contextual logging  
🌐 **Distributed Tracing** - OpenTelemetry integration  
⚡ **Enhanced Scheduling** - Simple extension methods  

This provides a robust foundation for scalable, observable, and maintainable background job processing in production environments.
