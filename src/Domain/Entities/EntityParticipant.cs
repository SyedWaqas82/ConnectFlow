namespace ConnectFlow.Domain.Entities;

public class EntityParticipant : BaseAuditableEntity, ITenantableEntity
{
    public int EntityId { get; set; }    // ID of the Lead, Deal, Person or Organization
    public EntityType EntityType { get; set; } // "Lead", "Deal", "Person" or "Organization"

    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public bool IsPrimary { get; set; } = false;

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}