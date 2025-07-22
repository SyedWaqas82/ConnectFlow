namespace ConnectFlow.Domain.Entities;

public class ChannelUser : BaseAuditableEntity, ITenantEntity
{
    public int ChannelId { get; set; } = default!;
    public Channel Channel { get; set; } = null!;

    public int UserId { get; set; } = default!;
    public TenantUser User { get; set; } = null!;

    public bool CanView { get; set; }
    public bool CanSend { get; set; }
    public bool CanManage { get; set; }

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
