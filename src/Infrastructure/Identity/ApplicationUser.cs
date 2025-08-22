using Microsoft.AspNetCore.Identity;

namespace ConnectFlow.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Avatar { get; set; }
    public string? JobTitle { get; set; }
    public string? Mobile { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public string Locale { get; set; } = "en-US";
    public bool IsActive { get; set; } = true;
    public string? Preferences { get; set; } // JSON string for user preferences
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public DateTimeOffset? DeactivatedAt { get; set; }

    // // Additional properties for token management
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }

    // Navigation properties
    public IList<TenantUser> TenantUsers { get; private set; } = new List<TenantUser>();
}