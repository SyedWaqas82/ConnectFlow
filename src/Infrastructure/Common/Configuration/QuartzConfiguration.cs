using Quartz;
using ConnectFlow.Infrastructure.Common.Jobs;
using Microsoft.Extensions.Hosting;
using ConnectFlow.Infrastructure.Common.Initializers;
using ConnectFlow.Infrastructure.Common.Providers;
using ConnectFlow.Infrastructure.Common.Models;

namespace ConnectFlow.Infrastructure.Common.Configuration;

/// <summary>
/// Configures production-ready job processing with Quartz.NET and PostgreSQL persistence
/// </summary>
/// <remarks>
/// This configuration provides:
/// 
/// 1. Enterprise-grade scheduled job execution
/// 2. Persistent PostgreSQL storage with proper database schema management
/// 3. High-availability clustering for fault tolerance and scalability
/// 4. Comprehensive metrics and monitoring integration
/// 5. Job recovery and error handling
/// 
/// Dashboard Access:
/// - Job monitoring: http://localhost:3000/d/quartz/quartz-jobs-dashboard?orgId=1
/// - Job details available in logs with structured information
/// 
/// Examples:
/// 
/// 1. Creating a production-ready job:
/// ```csharp
/// [DisallowConcurrentExecution]
/// public class EnterpriseJob : IJob
/// {
///     private readonly ILogger<EnterpriseJob> _logger;
///     private readonly IJobExecutionScopedService _service;
///     
///     public EnterpriseJob(ILogger<EnterpriseJob> logger, IJobExecutionScopedService service)
///     {
///         _logger = logger;
///         _service = service;
///     }
///     
///     public async Task Execute(IJobExecutionContext context)
///     {
///         try
///         {
///             var jobData = context.MergedJobDataMap;
///             var param = jobData.GetString("parameter") ?? "default";
///             await _service.ExecuteJobLogicAsync(param, context.CancellationToken);
///             _logger.LogInformation("Job {JobKey} completed successfully", context.JobDetail.Key);
///         }
///         catch (Exception ex)
///         {
///             _logger.LogError(ex, "Error executing job {JobKey}", context.JobDetail.Key);
///             throw; // Rethrow for Quartz to handle job failure logic
///         }
///     }
/// }
/// ```
/// 
/// 2. Triggering a job manually:
/// ```csharp
/// // Using the scheduler factory to avoid service locator anti-pattern
/// var scheduler = await _schedulerFactory.GetScheduler();
/// await scheduler.TriggerJob(new JobKey("JobName"), new JobDataMap { { "parameter", "value" } });
/// ```
/// </remarks>
public static class QuartzConfiguration
{
    public static void AddQuartzInfrastructure(this IHostApplicationBuilder builder)
    {
        // Get settings from configuration
        var quartzSettings = builder.Configuration.GetSection("QuartzSettings").Get<QuartzSettings>() ?? new QuartzSettings();
        var connectionString = builder.Configuration.GetConnectionString(quartzSettings.ConnectionStringName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string for Quartz is not configured. Please check your connection string settings.");
        }

        // Register Quartz settings with DI container
        builder.Services.Configure<QuartzSettings>(builder.Configuration.GetSection("QuartzSettings"));

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

            // Register jobs with triggers
            AddScheduledJobs(q, builder.Configuration);
        });



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

    /// <summary>
    /// Registers all scheduled jobs from configuration
    /// </summary>
    private static void AddScheduledJobs(IServiceCollectionQuartzConfigurator quartz, IConfiguration config)
    {
        // Add standard jobs

        // DataCleanupJob: runs daily at 2:00 AM
        AddJobWithCronTrigger<DataCleanupJob>(quartz, config, "0 0 2 * * ?", "Daily data cleanup job");

        // ReportGenerationJob (Weekly): runs every Monday at 8:00 AM
        AddJobWithCronTrigger<ReportGenerationJob>(quartz, config, "0 0 8 ? * MON", "Weekly report generation job", "Weekly");

        // ReportGenerationJob (Monthly): runs on the 1st of every month at 6:00 AM
        AddJobWithCronTrigger<ReportGenerationJob>(quartz, config, "0 0 6 1 * ?", "Monthly report generation job", "Monthly");

        // TestMetricsJob: runs every minute to generate metrics data
        AddJobWithCronTrigger<TestMetricsJob>(quartz, config, "0 * * * * ?", "Test metrics job");

        // LongRunningJob: runs every 3 seconds but takes 4-15 seconds to complete
        // This will definitely cause misfires because it takes longer than the trigger interval
        AddJobWithCronTrigger<LongRunningJob>(quartz, config, "0/3 * * * * ?", "Long running job that will cause misfires");
    }

    /// <summary>
    /// Add a scheduled job with its trigger from configuration or defaults
    /// </summary>
    private static void AddJobWithCronTrigger<T>(IServiceCollectionQuartzConfigurator quartz, IConfiguration config, string defaultCron, string defaultDesc, string? suffix = null) where T : IJob
    {
        var jobName = typeof(T).Name;
        if (!string.IsNullOrEmpty(suffix))
        {
            jobName = $"{jobName}.{suffix}";
        }

        var jobKey = new JobKey(jobName);
        var triggerKey = new TriggerKey($"Trigger.{jobName}");

        // Get configuration section for this job
        var jobConfig = config.GetSection($"QuartzJobs:{jobName}");

        // Get job parameters from configuration or use defaults
        var cronExpression = jobConfig["CronExpression"] ?? defaultCron;
        var description = jobConfig["Description"] ?? defaultDesc;
        var enabled = bool.TryParse(jobConfig["Enabled"], out var parsedEnabled) ? parsedEnabled : true;

        if (!enabled)
        {
            return;
        }

        // Define the job and tie it to our job class
        quartz.AddJob<T>(opts => opts
            .WithIdentity(jobKey)
            .WithDescription(description)
            .StoreDurably()
            .RequestRecovery()
        );

        // Create a trigger with specified CRON expression
        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(triggerKey)
            .WithDescription($"Trigger for {description}")
            .WithCronSchedule(cronExpression, builder => builder
                .WithMisfireHandlingInstructionFireAndProceed())
        );
    }
}