namespace ConnectFlow.Domain.Entities;

public class TenantUserRole : BaseAuditableEntity
{
    public string RoleName { get; set; } = string.Empty; // SuperAdmin, TenantAdmin, NonTenantAdmin
    public DateTimeOffset AssignedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public int? AssignedBy { get; set; } // ApplicationUserId who assigned this role
    public int TenantUserId { get; set; }
    public TenantUser TenantUser { get; set; } = null!;
}