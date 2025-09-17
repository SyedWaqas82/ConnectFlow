namespace ConnectFlow.Domain.Entities;

/// <summary>
/// Represents an audit log entry for tracking changes to entities
/// </summary>
public class ChangeLog : BaseAuditableEntity, ITenantableEntity
{
    /// <summary>
    /// The ID of the entity that was changed
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// The type of entity that was changed
    /// </summary>
    public EntityType EntityType { get; set; }

    /// <summary>
    /// The type of change that occurred
    /// </summary>
    public ChangeType ChangeType { get; set; }

    /// <summary>
    /// The name of the property that was changed (null for entity-level operations)
    /// </summary>
    public string? PropertyName { get; set; }

    /// <summary>
    /// The display name of the property for UI purposes
    /// </summary>
    public string? PropertyDisplayName { get; set; }

    /// <summary>
    /// The previous value before the change
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// The new value after the change
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Human-readable description of the change for UI display
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Additional metadata about the change (JSON format)
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// The tenant ID for multi-tenant isolation
    /// </summary>
    public int TenantId { get; set; }

    /// <summary>
    /// The tenant associated with this change log entry
    /// </summary>
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// The IP address from which the change was made
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// The user agent of the client that made the change
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Additional context about the change operation
    /// </summary>
    public string? Context { get; set; }
}