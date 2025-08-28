using Quartz;
using Microsoft.Extensions.Hosting;
using ConnectFlow.Infrastructure.Common.Models;
using ConnectFlow.Infrastructure.Quartz.Initializers;
using ConnectFlow.Infrastructure.Quartz.Providers;

namespace ConnectFlow.Infrastructure.Quartz;

/// <summary>
/// Configures production-ready job processing with Quartz.NET, PostgreSQL persistence, and enhanced BaseJob framework
/// </summary>
/// <remarks>
/// This configuration provides:
/// 
/// 1. Enterprise-grade scheduled job execution with BaseJob framework
/// 2. Persistent PostgreSQL storage with proper database schema management
/// 3. High-availability clustering for fault tolerance and scalability
/// 4. Comprehensive metrics and monitoring integration with automatic correlation tracking
/// 5. Job recovery and error handling with enhanced observability
/// 6. Automatic correlation ID generation and structured logging
/// 7. Context management for tenant/user-aware job execution
/// 
/// Enhanced Features:
/// - All jobs inherit from BaseJob for consistent behavior
/// - Automatic correlation ID tracking across job executions
/// - Comprehensive metrics collection (execution count, duration, errors)
/// - Structured logging with LogContext properties
/// - Distributed tracing with OpenTelemetry integration
/// - Tenant/user context initialization
/// 
/// Dashboard Access:
/// - Job monitoring: http://localhost:3000/d/quartz/quartz-jobs-dashboard?orgId=1
/// - Job details available in logs with structured information and correlation IDs
/// 
/// Job Scheduling:
/// Jobs are now scheduled using enhanced BaseJob extensions via JobSchedulingService.
/// This provides automatic correlation tracking, metrics collection, and context management.
/// 
/// Example job implementation:
/// ```csharp
/// [DisallowConcurrentExecution]
/// public class MyEnhancedJob : BaseJob
/// {
///     public MyEnhancedJob(ILogger<MyEnhancedJob> logger, IContextManager contextManager)
///         : base(logger, contextManager)
///     {
///     }
///     
///     protected override async Task ExecuteJobAsync(IJobExecutionContext context)
///     {
///         // Context is automatically initialized
///         // Correlation ID is automatically available
///         // Metrics are automatically collected
///         // Logging is automatically structured
///         
///         var correlationId = GetCurrentCorrelationId(context);
///         Logger.LogInformation("Processing with correlation {CorrelationId}", correlationId);
///         
///         // Your job logic here
///     }
/// }
/// ```
/// 
/// Manual job triggering:
/// ```csharp
/// await scheduler.ScheduleJobWithContextAsync<MyEnhancedJob>(
///     tenantId: 123,
///     applicationUserId: 456,
///     correlationId: "manual-trigger-001"
/// );
/// ```
/// </remarks>
public static class QuartzConfiguration
{
    public static void AddQuartzInfrastructure(this IHostApplicationBuilder builder)
    {
        // Get settings from configuration
        var quartzSettings = builder.Configuration.GetSection(QuartzSettings.SectionName).Get<QuartzSettings>() ?? new QuartzSettings();
        var connectionString = builder.Configuration.GetConnectionString(quartzSettings.ConnectionStringName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string for Quartz is not configured. Please check your connection string settings.");
        }

        // Register Quartz settings with DI container
        builder.Services.Configure<QuartzSettings>(builder.Configuration.GetSection(QuartzSettings.SectionName));

        if (quartzSettings.UsePersistentStore)
        {
            // Register Quartz schema provider to manage schema initialization
            builder.Services.AddSingleton<QuartzSchemaProvider>();
            builder.Services.AddHostedService<QuartzSchemaInitializer>();
        }

        // Configure Quartz with production best practices
        builder.Services.AddQuartz(q =>
        {
            // Basic scheduler properties
            q.SchedulerName = quartzSettings.SchedulerName;
            q.SchedulerId = quartzSettings.InstanceId;

            // Microsoft DI is the default in Quartz.NET 3.4+ so no need to configure it explicitly

            // Configure thread pool
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10; // Set based on your workload needs
            });

            // Production-grade persistent job store with PostgreSQL
            if (quartzSettings.UsePersistentStore)
            {
                q.UsePersistentStore(options =>
                {
                    options.UseProperties = quartzSettings.UseProperties;

                    options.UsePostgres(postgresOptions =>
                    {
                        postgresOptions.ConnectionString = connectionString;
                        postgresOptions.TablePrefix = quartzSettings.TablePrefix;
                    });

                    // Configure serialization
                    if (quartzSettings.SerializerType.Equals("json", StringComparison.OrdinalIgnoreCase))
                    {
                        options.UseNewtonsoftJsonSerializer();
                    }
                    else
                    {
                        options.UseBinarySerializer();
                    }

                    // Configure clustering for high availability
                    if (quartzSettings.UseClustering)
                    {
                        options.UseClustering(c =>
                        {
                            c.CheckinInterval = TimeSpan.FromSeconds(quartzSettings.ClusterCheckinIntervalSeconds);
                            c.CheckinMisfireThreshold = TimeSpan.FromSeconds(quartzSettings.MisfireThresholdSeconds);
                        });
                    }
                });
            }

            // Jobs will be scheduled using enhanced BaseJob extensions via JobSchedulingService
            // This allows for better correlation tracking, metrics, and context management
        });

        // Register the enhanced job scheduling service
        builder.Services.AddHostedService<JobSchedulingService>();



        // Register Quartz metrics provider - used by Prometheus for monitoring
        builder.Services.AddSingleton<QuartzMetricsProvider>();



        // Configure the Quartz hosted service (will be started after QuartzSchemaInitializerService)
        builder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
            options.AwaitApplicationStarted = true;
        });

        // Register Quartz schema and metrics initializers as hosted services
        // This will ensure tables are created after application startup and migrations
        builder.Services.AddHostedService<QuartzMetricsInitializer>();
    }
}