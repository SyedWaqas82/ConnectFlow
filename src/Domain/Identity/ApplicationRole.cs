using Microsoft.AspNetCore.Identity;

namespace ConnectFlow.Domain.Identity;

public class ApplicationRole : IdentityRole<int>
{
    public string Description { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty; // JSON array of permissions
    public string TenantId { get; set; } = string.Empty;
}
