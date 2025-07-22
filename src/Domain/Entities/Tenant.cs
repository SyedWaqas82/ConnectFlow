using ConnectFlow.Domain.Identity;

namespace ConnectFlow.Domain.Entities;

public class Tenant : BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public string Subdomain { get; set; } = default!;
    public string? CustomDomain { get; set; }
    public string? LogoUrl { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public string Locale { get; set; } = "en-US";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? TrialEndsAt { get; set; }

    public Subscription? Subscription { get; set; }
    public IList<Team> Teams { get; private set; } = new List<Team>();
    public IList<ApplicationUser> Users { get; private set; } = new List<ApplicationUser>();
}
