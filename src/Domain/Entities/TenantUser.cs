namespace ConnectFlow.Domain.Entities;

public class TenantUser : BaseAuditableEntity
{
    public int UserId { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; set; }
    public int? InvitedBy { get; set; } // UserId of inviter
    public TenantUserStatus Status { get; set; } = TenantUserStatus.Active;

    // Navigation properties
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public IList<TenantUserRole> TenantUserRoles { get; private set; } = new List<TenantUserRole>();
}
