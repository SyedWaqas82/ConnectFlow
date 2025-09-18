namespace ConnectFlow.Domain.Entities;

public class ProjectTask : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.NotStarted;
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? CompletionDate { get; set; }
    public bool IsDone { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int? ProjectPhaseId { get; set; }
    public ProjectPhase? ProjectPhase { get; set; }
    public int? AssigneeId { get; set; }
    public TenantUser? Assignee { get; set; }
    public int? ParentTaskId { get; set; }
    public ProjectTask? ParentTask { get; set; }
    public IList<ProjectTask> SubTasks { get; set; } = new List<ProjectTask>();

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ISoftDeleteableEntity implementation
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}