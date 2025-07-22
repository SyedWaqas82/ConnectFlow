namespace ConnectFlow.Domain.Entities;

public class Channel : BaseAuditableEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ChannelType Type { get; set; }
    public ChannelStatus Status { get; set; } = ChannelStatus.Active;

    // Credentials and Configuration (encrypted)
    public string Credentials { get; set; } = string.Empty;
    public string Configuration { get; set; } = string.Empty;

    // Limits and Usage
    public int? DailyMessageLimit { get; set; }
    public int? MonthlyMessageLimit { get; set; }
    public int CurrentDayMessageCount { get; set; }
    public int CurrentMonthMessageCount { get; set; }
    public DateTime? LastResetDate { get; set; }

    // Assignment and Access
    public int? DefaultAssigneeId { get; set; }
    public TenantUser? DefaultAssignee { get; set; }

    public int? DefaultTeamId { get; set; }
    public Team? DefaultTeam { get; set; }

    // Error Handling
    public string? LastError { get; set; }
    public DateTime? LastErrorAt { get; set; }
    public int ErrorCount { get; set; }

    // Collections
    public IList<Conversation> Conversations { get; private set; } = new List<Conversation>();
    public IList<Template> Templates { get; private set; } = new List<Template>();
    public IList<ChannelUser> ChannelUsers { get; private set; } = new List<ChannelUser>();

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
