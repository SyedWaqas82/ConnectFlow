namespace ConnectFlow.Domain.Entities;

public class TenantUser : BaseAuditableEntity, ISuspendibleEntity
{
    public int UserId { get; set; } = default!;
    public TenantUserStatus Status { get; set; } = TenantUserStatus.Active;
    public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LeftAt { get; set; }
    public int? InvitedBy { get; set; } // UserId of inviter

    // ISuspendibleEntity implementation
    public EntityStatus EntityStatus { get; set; } = EntityStatus.Active;
    public DateTimeOffset? SuspendedAt { get; set; }
    public DateTimeOffset? ResumedAt { get; set; }

    // Navigation properties
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public IList<TenantUserRole> TenantUserRoles { get; private set; } = new List<TenantUserRole>();
    //public IList<Lead> Leads { get; private set; } = new List<Lead>();
}