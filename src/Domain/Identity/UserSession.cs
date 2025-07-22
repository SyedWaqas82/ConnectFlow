namespace ConnectFlow.Domain.Identity;

public class UserSession : BaseAuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string DeviceId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset LastAccessedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }

    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
