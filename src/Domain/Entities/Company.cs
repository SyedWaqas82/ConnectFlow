namespace ConnectFlow.Domain.Entities;

public class Company : BaseAuditableEntity, ITenantEntity
{
    public string Name { get; set; } = null!;
    public string? Website { get; set; }
    public string? Industry { get; set; }

    public IList<Contact> Contacts { get; private set; } = new List<Contact>();

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
