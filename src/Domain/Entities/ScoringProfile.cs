namespace ConnectFlow.Domain.Entities;

public class ScoringProfile : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public EntityType TargetEntityType { get; set; }
    public int MaxScore { get; set; } = 100;
    public int MinScore { get; set; } = 0;
    public bool IsActive { get; set; } = false;
    public IList<ScoringGroup> ScoringGroups { get; private set; } = new List<ScoringGroup>();
    public IList<Pipeline> Pipelines { get; private set; } = new List<Pipeline>();

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
