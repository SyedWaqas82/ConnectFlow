namespace ConnectFlow.Domain.Entities;

public class AssignmentRuleCondition : BaseAuditableEntity, ITenantableEntity
{
    public int SortOrder { get; set; } = 0;
    public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;
    public int AssignmentRulesSetId { get; set; }
    public AssignmentRulesSet AssignmentRulesSet { get; set; } = null!;
    public AssignmentRuleField Field { get; set; }
    public RuleOperator Operator { get; set; }
    public required string Value { get; set; } // Value to compare against (stored as string, converted based on field type)
    public string? ValueTo { get; set; } // Additional value for range operations (like DateBetween)

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}