using System.ComponentModel.DataAnnotations;

namespace ConnectFlow.Domain.Entities;

public class TenantUserRole : BaseAuditableEntity, ITenantEntity
{
    [Key]
    public int ApplicationRoleId { get; set; } = default!;

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
