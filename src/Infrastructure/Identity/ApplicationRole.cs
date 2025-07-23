using Microsoft.AspNetCore.Identity;

namespace ConnectFlow.Infrastructure.Identity;

public class ApplicationRole : IdentityRole<int>
{
    public string Description { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
}