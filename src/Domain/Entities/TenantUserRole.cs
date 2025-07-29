namespace ConnectFlow.Domain.Entities;

public class TenantUserRole : BaseAuditableEntity
{
    public string RoleName { get; set; } = string.Empty; // SuperAdmin, TenantAdmin, NonTenantAdmin
    public bool IsActive { get; set; } = true;
    public DateTimeOffset AssignedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RevokedAt { get; set; }
    public int? AssignedBy { get; set; } // UserId who assigned this role
    public int TenantUserId { get; set; }
    public TenantUser TenantUser { get; set; } = null!;
}