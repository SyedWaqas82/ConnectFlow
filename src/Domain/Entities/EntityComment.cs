namespace ConnectFlow.Domain.Entities;

public class EntityComment : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity
{
    public required string Content { get; set; }

    public int EntityId { get; set; }    // ID of the Lead, Deal, Person or Organization
    public EntityType EntityType { get; set; } // "Lead", "Deal", "Person" or "Organization"

    // Author information
    public int AuthorId { get; set; }
    public TenantUser Author { get; set; } = null!;

    // ITenantEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ISoftDeleteEntity implementation
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}