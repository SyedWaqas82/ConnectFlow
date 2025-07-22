namespace ConnectFlow.Domain.Entities;

public class CustomField : BaseAuditableEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public CustomFieldType Type { get; set; }
    public EntityType EntityType { get; set; }
    public string? Options { get; set; } // JSON array for select/multi-select fields
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
