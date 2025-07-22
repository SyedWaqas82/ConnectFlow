namespace ConnectFlow.Domain.Entities;

public class TeamMember : BaseAuditableEntity, ITenantEntity
{
    public string TeamId { get; set; } = default!;
    public Team Team { get; set; } = null!;

    public int UserId { get; set; } = default!;
    public TenantUser User { get; set; } = null!;

    public bool IsTeamLead { get; set; }
    public string? Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
