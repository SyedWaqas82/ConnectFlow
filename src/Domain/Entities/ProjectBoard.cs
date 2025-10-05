namespace ConnectFlow.Domain.Entities;

public class ProjectBoard : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity
{
    public required string Name { get; set; }
    public int SortOrder { get; set; }
    public string? Description { get; set; }
    public IList<ProjectPhase> Phases { get; set; } = new List<ProjectPhase>();
    public IList<Project> Projects { get; set; } = new List<Project>();

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