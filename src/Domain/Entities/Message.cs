namespace ConnectFlow.Domain.Entities;

public class Message : BaseAuditableEntity, ITenantEntity
{
    public string Content { get; set; } = string.Empty;
    public MessageChannel Channel { get; set; }
    public string Type { get; set; } = "Text"; // Text, Image, File, etc.
    public MessageDirection Direction { get; set; }
    public MessageStatus Status { get; set; }
    public string? ExternalId { get; set; }
    public string? ExternalStatus { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTimeOffset? LastRetryAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }

    // Conversation Thread
    public int ConversationId { get; set; } = default!;
    public Conversation Conversation { get; set; } = null!;

    // Sender and Recipients
    public int? FromUserId { get; set; }
    public TenantUser? FromUser { get; set; }

    public int? ContactId { get; set; }
    public Contact? Contact { get; set; }

    public int? LeadId { get; set; }
    public Lead? Lead { get; set; }

    // Template
    public int? TemplateId { get; set; }
    public Template? Template { get; set; }

    // Attachments
    public IList<Attachment> Attachments { get; private set; } = new List<Attachment>();

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
