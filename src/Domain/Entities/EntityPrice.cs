namespace ConnectFlow.Domain.Entities;

public class EntityPrice : BaseAuditableEntity, ITenantableEntity
{
    public decimal UnitPrice { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? DirectCost { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Comment { get; set; }

    public int EntityId { get; set; }    // ID of the Lead, Deal, Person or Organization
    public EntityType EntityType { get; set; } // "Lead", "Deal", "Person" or "Organization"

    // TenentableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}