namespace ConnectFlow.Domain.Entities;

public class EntityChangeLog : BaseAuditableEntity, ITenantableEntity
{
    public int EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public ChangeType ChangeType { get; set; }
    public string? PropertyName { get; set; }
    public string? PropertyDisplayName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Metadata { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Context { get; set; }

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}