using ConnectFlow.Domain.Identity;

namespace ConnectFlow.Domain.Entities;

public class ConversationParticipant : BaseAuditableEntity, ITenantEntity
{
    public int ConversationId { get; set; } = default!;
    public Conversation Conversation { get; set; } = null!;

    public int UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    public DateTime? LastViewedAt { get; set; }
    public DateTime? LastParticipatedAt { get; set; }

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
