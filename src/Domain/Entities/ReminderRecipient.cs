namespace ConnectFlow.Domain.Entities;

public class ReminderRecipient : BaseAuditableEntity, ITenantEntity
{
    public int ReminderId { get; set; } = default!;
    public Reminder Reminder { get; set; } = null!;

    public int UserId { get; set; } = default!;
    public TenantUser User { get; set; } = null!;

    public bool IsDismissed { get; set; }
    public DateTime? DismissedAt { get; set; }

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
