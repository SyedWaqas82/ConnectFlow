using Microsoft.AspNetCore.Identity;

namespace ConnectFlow.Infrastructure.Identity;

public class ApplicationRole : IdentityRole<int>
{
    public ApplicationRole() : base() { }

    public ApplicationRole(string roleName) : base(roleName) { }

    public string Description { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
}