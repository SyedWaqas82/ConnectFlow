namespace ConnectFlow.Domain.Entities;

public class ScoringRuleCondition : BaseAuditableEntity, ITenantableEntity
{
    public int ScoringCriteriaId { get; set; }
    public ScoringCriteria ScoringCriteria { get; set; } = null!;
    public ScoringConditionType ConditionType { get; set; }
    public required string FieldName { get; set; }
    public ScoringOperator Operator { get; set; }
    public required string ComparisonValue { get; set; }
    public ScoringLogicOperator LogicOperator { get; set; }
    public int SortOrder { get; set; }

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}