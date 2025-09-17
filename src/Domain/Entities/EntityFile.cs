namespace ConnectFlow.Domain.Entities;

public class EntityFile : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity
{
    public string FileName { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public long FileSize { get; set; } // Size in bytes
    public string Url { get; set; } = null!; // URL or path to the file
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