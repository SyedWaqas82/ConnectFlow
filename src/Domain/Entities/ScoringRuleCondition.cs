namespace ConnectFlow.Domain.Entities;

public class ScoringRuleCondition : BaseAuditableEntity, ITenantableEntity
{
    public int ScoringCriteriaId { get; set; }
    public ScoringCriteria ScoringCriteria { get; set; } = null!;
    public ConditionType ConditionType { get; set; }
    public required string FieldName { get; set; }
    public RuleOperator Operator { get; set; }
    public required string ComparisonValue { get; set; }
    public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;
    public int SortOrder { get; set; }

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}