namespace ConnectFlow.Domain.Entities;

public class SequenceStep : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public SequenceStepType StepType { get; set; }
    public int StepOrder { get; set; }
    public int DelayDays { get; set; } = 0;
    public int DelayMinutes { get; set; } = 0;
    public bool IncludeWeekends { get; set; } = true;
    public DateTimeOffset? ScheduledAt { get; set; }
    public string Subject { get; set; } = string.Empty;
    public ActivityType Type { get; set; }
    public string Note { get; set; } = string.Empty;
    public ActivityPriority Priority { get; set; } = ActivityPriority.None;
    public int ActivityOwnerId { get; set; }
    public int SequenceId { get; set; }
    public Sequence Sequence { get; set; } = null!;
    public IList<EntitySequenceEnrollment> CurrentEnrollments { get; private set; } = new List<EntitySequenceEnrollment>(); // Enrollments currently at this step
    public IList<EntityActivity> Activities { get; private set; } = new List<EntityActivity>(); // Activities created from this step

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ISoftDeleteableEntity implementation
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}