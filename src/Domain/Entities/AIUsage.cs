namespace ConnectFlow.Domain.Entities;

public class AIUsage : BaseAuditableEntity, ITenantEntity
{
    public string ModelId { get; set; } = default!;
    public string ModelName { get; set; } = default!;
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public decimal Cost { get; set; }
    public AIRequestType RequestType { get; set; } = default!;
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }

    // User who made the request
    public int UserId { get; set; } = default!;
    public TenantUser User { get; set; } = null!;

    // Associated entities
    public int? ConversationId { get; set; }
    public Conversation? Conversation { get; set; }

    public int? MessageId { get; set; }
    public Message? Message { get; set; }

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}