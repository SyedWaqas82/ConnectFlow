namespace ConnectFlow.Infrastructure.Common.Models;

/// <summary>
/// Settings for Quartz.NET configuration
/// </summary>
public class QuartzSettings
{
    public const string SectionName = "QuartzSettings";
    /// <summary>
    /// Name of the connection string to use for Quartz database
    /// </summary>
    public string ConnectionStringName { get; set; } = "ConnectFlowDb";

    /// <summary>
    /// Scheduler instance name, should be unique per application
    /// </summary>
    public string SchedulerName { get; set; } = "ConnectFlowScheduler";

    /// <summary>
    /// Instance ID, should be unique per server in a cluster
    /// Default is AUTO which uses the machine name
    /// </summary>
    public string InstanceId { get; set; } = "AUTO";

    /// <summary>
    /// Whether to use persistent store for jobs and triggers
    /// </summary>
    public bool UsePersistentStore { get; set; } = true;

    /// <summary>
    /// Whether to use properties for job data map instead of BLOB
    /// </summary>
    public bool UseProperties { get; set; } = true;

    /// <summary>
    /// Table prefix for Quartz tables
    /// </summary>
    public string TablePrefix { get; set; } = "QRTZ_";

    /// <summary>
    /// Whether to use clustering for high availability
    /// </summary>
    public bool UseClustering { get; set; } = false;

    /// <summary>
    /// Cluster check-in interval in seconds
    /// </summary>
    public int ClusterCheckinIntervalSeconds { get; set; } = 15;

    /// <summary>
    /// Maximum number of connections in the connection pool
    /// </summary>
    public int MaxConnections { get; set; } = 5;

    /// <summary>
    /// Misfire threshold in seconds
    /// </summary>
    public int MisfireThresholdSeconds { get; set; } = 60;

    /// <summary>
    /// Maximum number of misfired triggers to handle at a time
    /// </summary>
    public int MaxMisfireCount { get; set; } = 20;

    /// <summary>
    /// Job serializer type: 'binary' or 'json'
    /// </summary>
    public string SerializerType { get; set; } = "json";

    /// <summary>
    /// Whether to validate schema on startup
    /// </summary>
    public bool ValidateSchema { get; set; } = true;
}
