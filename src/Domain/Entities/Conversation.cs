// namespace ConnectFlow.Domain.Entities;

// public class Conversation : BaseAuditableEntity, ITenantEntity
// {
//     public string Title { get; set; } = string.Empty;
//     public ConversationStatus Status { get; set; } = ConversationStatus.Open;
//     public DateTimeOffset? LastMessageAt { get; set; }
//     public DateTimeOffset? LastAssignedAt { get; set; }
//     public bool IsArchived { get; set; }
//     public DateTimeOffset? ArchivedAt { get; set; }
//     public string? ArchivedById { get; set; }
//     public TenantUser? ArchivedBy { get; set; }

//     // Channel
//     public string ChannelId { get; set; } = string.Empty;
//     public Channel Channel { get; set; } = null!;

//     // Contact/Lead Reference
//     public int? ContactId { get; set; }
//     public Contact? Contact { get; set; }

//     public int? LeadId { get; set; }
//     public Lead? Lead { get; set; }

//     // Assignment
//     public int? AssigneeId { get; set; }
//     public TenantUser? Assignee { get; set; }

//     // Team Assignment
//     public int? TeamId { get; set; }
//     public Team? Team { get; set; }

//     // Collections
//     public IList<Message> Messages { get; private set; } = new List<Message>();
//     public IList<Tag> Tags { get; private set; } = new List<Tag>();
//     public IList<ConversationParticipant> Participants { get; private set; } = new List<ConversationParticipant>();

//     // Tenant
//     public int TenantId { get; set; } = default!;
//     public Tenant Tenant { get; set; } = null!;
// }
