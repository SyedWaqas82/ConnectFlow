namespace ConnectFlow.Domain.Entities;

public class Label : BaseAuditableEntity, ITenantableEntity
{
    public required string Name { get; set; }
    public required string Color { get; set; } // Store hex color code like "#FF5733"
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public EntityType EntityType { get; set; } // Indicates which entity this label is for

    // ITenantEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // Navigation properties
    public IList<EntityLabel> Labels { get; private set; } = new List<EntityLabel>();
}