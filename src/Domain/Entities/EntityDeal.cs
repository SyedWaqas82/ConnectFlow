namespace ConnectFlow.Domain.Entities;

public class EntityDeal : BaseAuditableEntity, ITenantableEntity
{
    public int EntityId { get; set; }    // ID of the Lead, Deal, Person or Organization
    public EntityType EntityType { get; set; } // "Lead", "Deal", "Person" or "Organization"

    public int DealId { get; set; }
    public Deal Deal { get; set; } = null!;

    // ITenantEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}