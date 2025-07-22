using Microsoft.AspNetCore.Identity;

namespace ConnectFlow.Domain.Identity;

public class ApplicationUser : IdentityUser<int>, ITenantEntity
{
    public string? DisplayName { get; set; }

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}