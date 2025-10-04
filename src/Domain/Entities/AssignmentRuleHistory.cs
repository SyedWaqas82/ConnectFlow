namespace ConnectFlow.Domain.Entities;

public class AssignmentRuleHistory : BaseAuditableEntity, ITenantableEntity
{
    public int AssignmentRuleId { get; set; }
    public AssignmentRule AssignmentRule { get; set; } = null!;
    public AssignmentRuleEntityType EntityType { get; set; }
    public int EntityId { get; set; }
    public required string EntityTitle { get; set; }
    public int? PreviousAssignedUserId { get; set; }
    public TenantUser PreviousAssignedUser { get; set; } = null!;
    public int? NewAssignedUserId { get; set; }
    public TenantUser NewAssignedUser { get; set; } = null!;
    public AssignmentRuleExecutionResult ExecutionResult { get; set; }
    public DateTimeOffset ExecutionDate { get; set; } = DateTimeOffset.UtcNow;
    public long ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
    public int? TriggeredByUserId { get; set; }
    public TenantUser TriggeredByUser { get; set; } = null!;
    public AssignmentTriggerEvent TriggerEventSource { get; set; }

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}