using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Domain.Entities;

public class Project : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity, IFileableEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    public TenantUser Owner { get; set; } = null!;
    public int? OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public int? PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public ProjectPriority Priority { get; set; } = ProjectPriority.Normal;
    public bool IsArchived { get; set; }
    public int ProjectBoardId { get; set; }
    public ProjectBoard ProjectBoard { get; set; } = null!;
    public int? ProjectPhaseId { get; set; }
    public ProjectPhase ProjectPhase { get; set; } = null!;
    public IList<ProjectDeal> ProjectDeals { get; private set; } = new List<ProjectDeal>();

    // Interface implementations
    [NotMapped]
    public EntityType EntityType => EntityType.Project;
    [NotMapped]
    public IList<EntityFile> Files { get; set; } = new List<EntityFile>();

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