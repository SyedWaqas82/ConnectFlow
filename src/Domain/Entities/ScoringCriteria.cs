namespace ConnectFlow.Domain.Entities;

public class ScoringCriteria : BaseAuditableEntity, ITenantableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int ScoringGroupId { get; set; }
    public ScoringGroup ScoringGroup { get; set; } = null!;
    public int SortOrder { get; set; }
    public IList<ScoringRuleCondition> Conditions { get; private set; } = new List<ScoringRuleCondition>();

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}