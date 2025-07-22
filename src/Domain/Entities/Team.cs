using ConnectFlow.Domain.Identity;

namespace ConnectFlow.Domain.Entities;

public class Team : BaseAuditableEntity, ITenantEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public IList<ApplicationUser> Members { get; private set; } = new List<ApplicationUser>();

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
