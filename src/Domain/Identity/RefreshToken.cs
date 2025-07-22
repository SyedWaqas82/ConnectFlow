namespace ConnectFlow.Domain.Identity;

public class RefreshToken : BaseAuditableEntity
{
    public string Token { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty;
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTimeOffset ExpiryDate { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string TenantId { get; set; } = string.Empty;
}
