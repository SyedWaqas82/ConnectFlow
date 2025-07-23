namespace ConnectFlow.Domain.Entities;

public class ChannelAccount : BaseAuditableEntity, ITenantEntity
{
    public string Name { get; set; } = default!;
    public ChannelType Type { get; set; }
    public string AccountIdentifier { get; set; } = default!; // Phone number for WhatsApp, Page ID for Facebook, etc.
    public string? DisplayName { get; set; }
    public ChannelStatus Status { get; set; }

    // Encrypted credentials
    public string EncryptedCredentials { get; set; } = default!;
    public string EncryptionKeyId { get; set; } = default!; // Reference to the key used for encryption

    // Provider specific
    public string Provider { get; set; } = default!; // Twilio, Meta, SendGrid, etc.
    public string? ProviderAccountId { get; set; }
    public string? ProviderMetadata { get; set; } // JSON string with provider-specific data

    // Configuration
    public string? WebhookUrl { get; set; }
    public string? WebhookSecret { get; set; }
    public bool WebhookEnabled { get; set; }
    public DateTimeOffset? LastWebhookCall { get; set; }

    // Channel reference
    public int ChannelId { get; set; } = default!;
    public Channel Channel { get; set; } = null!;

    // Integration status
    public bool IsVerified { get; set; }
    public DateTimeOffset? LastVerifiedAt { get; set; }
    public string? VerificationError { get; set; }
    public int FailedAttempts { get; set; }
    public DateTimeOffset? LastFailureAt { get; set; }

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
