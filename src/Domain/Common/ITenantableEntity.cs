namespace ConnectFlow.Domain.Common;

public interface ITenantableEntity
{
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }
}