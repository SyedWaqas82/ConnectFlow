namespace ConnectFlow.Domain.Entities;

public class TenantUserRole : BaseAuditableEntity
{
    public string RoleName { get; set; } = string.Empty; // SuperAdmin, TenantAdmin, NonTenantAdmin
    public bool IsActive { get; set; } = true;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public int? AssignedBy { get; set; } // UserId who assigned this role

    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public int TenantUserId { get; set; }
    public TenantUser TenantUser { get; set; } = null!;
}