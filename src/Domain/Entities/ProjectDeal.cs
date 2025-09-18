namespace ConnectFlow.Domain.Entities;

public class ProjectDeal : BaseAuditableEntity, ITenantableEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int DealId { get; set; }
    public Deal Deal { get; set; } = null!;

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}