namespace ConnectFlow.Domain.Entities;

public class Contact : BaseAuditableEntity, ITenantEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public int? CompanyId { get; set; }
    public Company? Company { get; set; }

    public ContactScore Score { get; set; } = new();
    public IList<Appointment> Appointments { get; private set; } = new List<Appointment>();

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
