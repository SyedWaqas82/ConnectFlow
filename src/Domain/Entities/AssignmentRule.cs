namespace ConnectFlow.Domain.Entities;

public class AssignmentRule : BaseAuditableEntity, ITenantableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public AssignmentRuleEntityType EntityType { get; set; }
    public AssignmentTriggerEvent TriggerEvent { get; set; } = AssignmentTriggerEvent.OnCreate;
    public int AssignToUserId { get; set; }
    public TenantUser AssignToUser { get; set; } = null!;
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = false;
    public DateTimeOffset? ActiveFrom { get; set; }
    public DateTimeOffset? DeactivatedAt { get; set; }
    public DateTimeOffset? LastExecutedAt { get; set; }
    public IList<AssignmentRulesSet> RulesSets { get; private set; } = new List<AssignmentRulesSet>();
    public IList<AssignmentRuleHistory> AssignmentHistories { get; private set; } = new List<AssignmentRuleHistory>();

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}