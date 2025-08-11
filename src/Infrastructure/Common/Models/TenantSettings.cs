namespace ConnectFlow.Infrastructure.Common.Models;

public class TenantSettings
{
    public const string SectionName = "TenantSettings";
    public string HeaderName { get; set; } = "X-Tenant-Id";
}