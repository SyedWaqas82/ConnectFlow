namespace ConnectFlow.Application.Common.Models;

public class RedisSettings
{
    public string Configuration { get; set; } = "localhost:6379";
    public string InstanceName { get; set; } = "ConnectFlow:";
}