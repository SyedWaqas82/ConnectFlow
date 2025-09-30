namespace ConnectFlow.Domain.Entities;

public class AssignmentRulesSet : BaseAuditableEntity, ITenantableEntity
{
    public int SortOrder { get; set; } = 0;
    public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;
    public int AssignmentRuleId { get; set; }
    public AssignmentRule AssignmentRule { get; set; } = null!;
    public IList<AssignmentRuleCondition> Conditions { get; private set; } = new List<AssignmentRuleCondition>();

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}