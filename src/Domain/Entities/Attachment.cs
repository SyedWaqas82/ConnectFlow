namespace ConnectFlow.Domain.Entities;

public class Attachment : BaseAuditableEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;

    public int MessageId { get; set; } = default!;
    public Message Message { get; set; } = null!;

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
