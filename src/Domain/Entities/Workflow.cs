namespace ConnectFlow.Domain.Entities;

public class Workflow : BaseAuditableEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string TriggerType { get; set; } = string.Empty;
    public string TriggerCondition { get; set; } = string.Empty;

    public IList<Task> Tasks { get; private set; } = new List<Task>();
    public IList<Trigger> Triggers { get; private set; } = new List<Trigger>();

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
