namespace ConnectFlow.Domain.Entities;

public class DealInstallment : BaseAuditableEntity, ITenantableEntity
{
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset BillingDate { get; set; }
    public decimal Amount { get; set; }
    public int SortOrder { get; set; }
    public int DealId { get; set; }
    public Deal Deal { get; set; } = null!;

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}