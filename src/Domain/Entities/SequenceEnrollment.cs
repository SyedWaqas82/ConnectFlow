namespace ConnectFlow.Domain.Entities;

public class EntitySequenceEnrollment : BaseAuditableEntity, ITenantableEntity
{
    public SequenceEnrollmentStatus Status { get; set; } = SequenceEnrollmentStatus.Active;
    public int EntityId { get; set; }    // ID of the Lead, Deal, Person or Organization
    public EntityType EntityType { get; set; } // "Lead", "Deal", "Person" or "Organization"
    public int? CurrentStepId { get; set; }
    public SequenceStep? CurrentStep { get; set; }
    public DateTimeOffset EnrolledAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    public int SequenceId { get; set; }
    public Sequence Sequence { get; set; } = null!;

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}