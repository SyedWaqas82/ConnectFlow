using ConnectFlow.Domain.Identity;

namespace ConnectFlow.Domain.Entities;

public class Task : BaseAuditableEntity, ITenantEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskState Status { get; set; } = TaskState.Open;
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? CompletedDate { get; set; }

    public int? AssigneeId { get; set; }
    public ApplicationUser? Assignee { get; set; }

    public int? ContactId { get; set; }
    public Contact? Contact { get; set; }

    public int? CompanyId { get; set; }
    public Company? Company { get; set; }

    public int? LeadId { get; set; }
    public Lead? Lead { get; set; }

    public int? WorkflowId { get; set; }
    public Workflow? Workflow { get; set; }

    public IList<Reminder> Reminders { get; private set; } = new List<Reminder>();
    public IList<Tag> Tags { get; private set; } = new List<Tag>();

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}