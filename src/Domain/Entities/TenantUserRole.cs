using ConnectFlow.Domain.Identity;

namespace ConnectFlow.Domain.Entities;

public class TenantUserRole : BaseAuditableEntity, ITenantEntity
{
    public string RoleId { get; set; } = default!;
    public ApplicationRole Role { get; set; } = null!;

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
