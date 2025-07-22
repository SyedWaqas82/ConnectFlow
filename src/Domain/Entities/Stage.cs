namespace ConnectFlow.Domain.Entities;

public class Stage : BaseAuditableEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public Colour Color { get; set; } = Colour.Green;
    public decimal WinProbability { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    public int PipelineId { get; set; } = default!;
    public Pipeline Pipeline { get; set; } = null!;

    public IList<Lead> Leads { get; private set; } = new List<Lead>();

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
