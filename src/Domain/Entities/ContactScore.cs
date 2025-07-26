namespace ConnectFlow.Domain.Entities;

public class ContactScore : BaseAuditableEntity, ITenantEntity, ISoftDelete
{
    public int EngagementScore { get; set; }
    public int QualificationScore { get; set; }
    public DateTimeOffset LastInteraction { get; set; }
    public int InteractionCount { get; set; }
    public int TotalScore => EngagementScore + QualificationScore;

    public int ContactId { get; set; } = default!;
    public Contact Contact { get; set; } = null!;

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;

    // Soft delete properties
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}