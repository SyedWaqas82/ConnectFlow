using System.ComponentModel.DataAnnotations;

namespace ConnectFlow.Domain.Entities;

public class TenantUser : BaseAuditableEntity, ITenantEntity
{
    [Key]
    public int ApplicationUserId { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }

    // Tenant-specific user settings
    public string? DisplayName { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public string? Phone { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public string Locale { get; set; } = "en-US";

    // Role assignments for this tenant
    public IList<TenantUserRole> Roles { get; private set; } = new List<TenantUserRole>();

    // Team memberships
    public IList<TeamMember> TeamMemberships { get; private set; } = new List<TeamMember>();

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
