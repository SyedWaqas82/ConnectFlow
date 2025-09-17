namespace ConnectFlow.Domain.Entities;

public class EntityActivity : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity
{
    public required string Subject { get; set; }
    public ActivityType Type { get; set; }
    public string Note { get; set; } = string.Empty;
    public bool Done { get; set; } = false;
    public DateTimeOffset? StartAt { get; set; }
    public DateTimeOffset? EndAt { get; set; }
    public ActivityPriority Priority { get; set; } = ActivityPriority.None;
    public string Location { get; set; } = string.Empty;
    public string ConferenceUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; // Display on calendar details
    public CalendarVisibilityStatus VisibilityOnCalendar { get; set; } = CalendarVisibilityStatus.Free;
    public int? AssignedById { get; set; }
    public TenantUser AssignedBy { get; set; } = null!;
    public int? AssignedToId { get; set; }
    public TenantUser AssignedTo { get; set; } = null!;
    public int EntityId { get; set; }    // ID of the Lead, Deal, Person or Organization
    public EntityType EntityType { get; set; } // "Lead", "Deal", "Person" or "Organization"
    public IList<EntityActivityParticipant> Participants { get; private set; } = new List<EntityActivityParticipant>();

    // ITenantEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ISoftDeleteEntity implementation
    public bool IsDeleted { get; set; } = false; // Soft delete flag
    public DateTimeOffset? DeletedAt { get; set; } = null!; // When the entity was deleted
    public int? DeletedBy { get; set; } = null!; // User who deleted the entity
}