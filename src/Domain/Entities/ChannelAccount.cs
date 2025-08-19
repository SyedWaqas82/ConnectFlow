namespace ConnectFlow.Domain.Entities;

public class ChannelAccount : BaseAuditableEntity, ITenantEntity, ISoftDelete
{
    public ChannelType Type { get; set; } // Type of channel (WhatsApp, Facebook, Instagram, ...)
    public string? ProviderAccountId { get; set; } // Unique identifier from the provider (e.g. WhatsApp business account id, Facebook page id)
    public string? DisplayName { get; set; } // Human friendly name shown in UI
    public string? Contact { get; set; } // Contact phone (for WhatsApp) or page handle
    public string? SettingsJson { get; set; } // Arbitrary provider-specific settings serialized as JSON
    public ChannelStatus Status { get; set; } = ChannelStatus.Active; // Status of the channel account

    // Tenant
    public int TenantId { get; set; } = default!; // Foreign key to Tenant
    public Tenant Tenant { get; set; } = null!; // Navigation property to Tenant
    public bool IsDeleted { get; set; } = false; // Soft delete flag
    public DateTimeOffset? DeletedAt { get; set; } = null!; // When the entity was deleted
    public int? DeletedBy { get; set; } = null!; // User who deleted the entity
}