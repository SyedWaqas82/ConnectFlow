namespace ConnectFlow.Domain.Entities;

public class EntityDocument : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity
{
    public required string FileName { get; set; }
    public string FileType { get; set; } = null!;
    public string FileUrl { get; set; } = null!;

    public int EntityId { get; set; }    // ID of the Lead, Deal, Person or Organization
    public EntityType EntityType { get; set; } // "Lead", "Deal", "Person" or "Organization"

    // ITenantEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ISoftDeleteEntity implementation
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}