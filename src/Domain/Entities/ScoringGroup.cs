namespace ConnectFlow.Domain.Entities;

public class ScoringGroup : BaseAuditableEntity, ITenantableEntity, ISuspendibleEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Points { get; set; }
    public int SortOrder { get; set; }
    public int ScoringProfileId { get; set; }
    public ScoringProfile ScoringProfile { get; set; } = null!;
    public IList<ScoringCriteria> ScoringCriterias { get; private set; } = new List<ScoringCriteria>();

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ISuspendibleEntity implementation
    public EntityStatus EntityStatus { get; set; } = EntityStatus.Active;
    public DateTimeOffset? SuspendedAt { get; set; }
    public DateTimeOffset? ResumedAt { get; set; }
}