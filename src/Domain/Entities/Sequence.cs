namespace ConnectFlow.Domain.Entities;

public class Sequence : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public EntityType TargetType { get; set; }
    public int OwnerId { get; set; }
    public TenantUser Owner { get; set; } = null!;
    public string? Settings { get; set; } // JSON settings for the sequence
    public IList<SequenceStep> Steps { get; private set; } = new List<SequenceStep>();
    public IList<EntitySequenceEnrollment> Enrollments { get; private set; } = new List<EntitySequenceEnrollment>();

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ISoftDeleteableEntity implementation
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }

    // ISuspendibleEntity implementation
    public EntityStatus EntityStatus { get; set; } = EntityStatus.Active;
    public DateTimeOffset? SuspendedAt { get; set; }
    public DateTimeOffset? ResumedAt { get; set; }
}