namespace ConnectFlow.Domain.Entities;

public class Tenant : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Domain { get; set; } = string.Empty; // e.g., company.yoursaas.com
    public string? Avatar { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Settings { get; set; } // JSON string for tenant-specific settings
    public DateTimeOffset? DeactivatedAt { get; set; }

    // Navigation properties
    public IList<TenantUser> TenantUsers { get; private set; } = new List<TenantUser>();
    public IList<Subscription> Subscriptions { get; private set; } = new List<Subscription>();
}