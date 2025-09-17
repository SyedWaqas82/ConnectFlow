namespace ConnectFlow.Domain.Entities;

public class TenantUser : BaseAuditableEntity, ISuspendibleEntity
{
    public int ApplicationUserId { get; set; } = default!;
    public TenantUserStatus Status { get; set; } = TenantUserStatus.Active;
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? LeftAt { get; set; }
    public int? InvitedBy { get; set; } // ApplicationUserId of inviter

    // ISuspendibleEntity implementation
    public EntityStatus EntityStatus { get; set; } = EntityStatus.Active;
    public DateTimeOffset? SuspendedAt { get; set; }
    public DateTimeOffset? ResumedAt { get; set; }

    // Navigation properties
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public IList<TenantUserRole> TenantUserRoles { get; private set; } = new List<TenantUserRole>();
    public IList<Lead> Leads { get; private set; } = new List<Lead>();
    public IList<Deal> Deals { get; private set; } = new List<Deal>();
    public IList<Note> Notes { get; private set; } = new List<Note>();
    public IList<EntityActivity> Activities { get; private set; } = new List<EntityActivity>(); // Activities assigned to this user
}