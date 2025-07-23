using Microsoft.AspNetCore.Identity;

namespace ConnectFlow.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public DateTime? DeactivatedAt { get; set; }

    // // Additional properties for token management
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}