using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Domain.Entities;

public class ProjectPhase : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity, IActivatableEntity
{
    public required string Name { get; set; }
    public int SortOrder { get; set; }
    public int ProjectBoardId { get; set; }
    public ProjectBoard ProjectBoard { get; set; } = null!;
    public IList<Project> Projects { get; set; } = new List<Project>();
    public IList<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

    [NotMapped]
    public EntityType EntityType => EntityType.ProjectPhase;
    [NotMapped]
    public IList<EntityActivity> Activities { get; set; } = new List<EntityActivity>();

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