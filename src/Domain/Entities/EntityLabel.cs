namespace ConnectFlow.Domain.Entities;

public class EntityLabel : BaseAuditableEntity
{
    public int EntityId { get; set; }    // ID of the Lead, Deal, Person or Organization
    public EntityType EntityType { get; set; } // "Lead", "Deal", "Person" or "Organization"

    public int LabelId { get; set; }
    public Label Label { get; set; } = null!;

    public DateTimeOffset AssignedAt { get; set; } = DateTimeOffset.UtcNow;
    public int? AssignedBy { get; set; } // User who assigned this label
    public TenantUser AssignedByUser { get; set; } = null!; // Navigation property to the user who assigned the label
}