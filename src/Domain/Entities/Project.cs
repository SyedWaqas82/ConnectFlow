using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Domain.Entities;

public class Project : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity, ILabelableEntity, INoteableEntity, IFileableEntity, IDocumentableEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    public TenantUser Owner { get; set; } = null!;
    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; }
    public int? PersonId { get; set; }
    public Person? Person { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public ProjectPriority Priority { get; set; } = ProjectPriority.Normal;
    public bool IsArchived { get; set; }
    public int ProjectBoardId { get; set; }
    public ProjectBoard ProjectBoard { get; set; } = null!;
    public int? PhaseId { get; set; }
    public ProjectPhase? Phase { get; set; }
    public IList<ProjectDeal> ProjectDeals { get; private set; } = new List<ProjectDeal>();

    // Interface implementations
    [NotMapped]
    public EntityType EntityType => EntityType.Project;
    [NotMapped]
    public IList<EntityLabel> Labels { get; set; } = new List<EntityLabel>();
    [NotMapped]
    public IList<Note> Notes { get; set; } = new List<Note>();
    [NotMapped]
    public IList<EntityFile> Files { get; set; } = new List<EntityFile>();
    [NotMapped]
    public IList<EntityDocument> Documents { get; set; } = new List<EntityDocument>();

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

    // IChangeLogableEntity implementation
    public IList<string> GetLoggableFields()
    {
        return new List<string>
        {
            nameof(Title),
            nameof(Description),
            nameof(OwnerId),
            nameof(OrganizationId),
            nameof(PersonId),
            nameof(Status),
            nameof(Priority),
            nameof(PhaseId),
            nameof(IsArchived),
            nameof(IsDeleted),
            nameof(EntityStatus)
        };
    }

    public string GetPropertyDisplayName(string propertyName)
    {
        return propertyName switch
        {
            nameof(Title) => "Project Title",
            nameof(Description) => "Description",
            nameof(OwnerId) => "Project Owner",
            nameof(OrganizationId) => "Client Organization",
            nameof(PersonId) => "Primary Contact",
            nameof(Status) => "Project Status",
            nameof(Priority) => "Priority",
            nameof(PhaseId) => "Current Phase",
            nameof(IsArchived) => "Archived",
            nameof(IsDeleted) => "Deleted",
            nameof(EntityStatus) => "Status",
            _ => propertyName
        };
    }

    public string FormatValueForDisplay(string propertyName, object? value)
    {
        if (value == null) return "Not Set";

        return propertyName switch
        {
            nameof(IsArchived) or nameof(IsDeleted) when value is bool b => b ? "Yes" : "No",
            nameof(Status) when value is ProjectStatus status => status.ToString(),
            nameof(Priority) when value is ProjectPriority priority => priority.ToString(),
            nameof(EntityStatus) when value is EntityStatus entityStatus => entityStatus.ToString(),
            _ => value.ToString() ?? "Not Set"
        };
    }
}