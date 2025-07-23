namespace ConnectFlow.Domain.Entities;

public class ConversationParticipant : BaseAuditableEntity, ITenantEntity
{
    public int ConversationId { get; set; } = default!;
    public Conversation Conversation { get; set; } = null!;

    public int UserId { get; set; } = default!;
    public TenantUser User { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    public DateTimeOffset? LastViewedAt { get; set; }
    public DateTimeOffset? LastParticipatedAt { get; set; }

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
