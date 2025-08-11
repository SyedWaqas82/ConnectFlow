namespace ConnectFlow.Infrastructure.Common.Models;

public class RedisSettings
{
    public const string SectionName = "RedisSettings";
    public string InstanceName { get; set; } = "ConnectFlow:";
}