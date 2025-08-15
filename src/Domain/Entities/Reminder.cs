// namespace ConnectFlow.Domain.Entities;

// public class Reminder : BaseAuditableEntity, ITenantEntity
// {
//     public string Title { get; set; } = string.Empty;
//     public string Description { get; set; } = string.Empty;
//     public DateTimeOffset DueDate { get; set; }
//     public ReminderType Type { get; set; }
//     public ReminderChannel Channel { get; set; }
//     public bool IsRecurring { get; set; }
//     public string? RecurrencePattern { get; set; }
//     public string? RecurrenceConfig { get; set; }
//     public bool IsDismissed { get; set; }
//     public DateTimeOffset? DismissedAt { get; set; }
//     public int? DismissedById { get; set; }
//     public TenantUser? DismissedBy { get; set; }
//     public bool IsCompleted { get; set; }
//     public DateTimeOffset? CompletedAt { get; set; }
//     public int? CompletedById { get; set; }
//     public TenantUser? CompletedBy { get; set; }

//     // WorkTask Reference
//     public int WorkTaskId { get; set; } = default!;
//     public WorkTask WorkTask { get; set; } = null!;

//     // Primary Assignee
//     public int? AssigneeId { get; set; }
//     public TenantUser? Assignee { get; set; }

//     // Additional Recipients
//     public IList<ReminderRecipient> Recipients { get; private set; } = new List<ReminderRecipient>();

//     // Tenant
//     public int TenantId { get; set; } = default!;
//     public Tenant Tenant { get; set; } = null!;
// }
