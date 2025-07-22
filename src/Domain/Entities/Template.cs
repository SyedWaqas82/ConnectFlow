namespace ConnectFlow.Domain.Entities;

public class Template : BaseAuditableEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Text, Media, Interactive, etc.
    public string Language { get; set; } = "en";
    public string Category { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string Status { get; set; } = "Draft";

    public int ChannelId { get; set; } = default!;
    public Channel Channel { get; set; } = null!;

    public IList<Message> Messages { get; private set; } = new List<Message>();

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
