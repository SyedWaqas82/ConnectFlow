using System.Text.Json;

namespace ConnectFlow.Domain.Entities;

public class Tenant : BaseAuditableEntity
{
    public required string Name { get; set; }
    public string? Domain { get; set; } = string.Empty; // e.g., company.yoursaas.com
    public string Email { get; set; } = string.Empty;
    public string PaymentProviderCustomerId { get; set; } = string.Empty;
    public string? Settings { get; set; } = JsonSerializer.Serialize(new
    {
        ShowPopupOnDealWin = true,
        ScheduleActivityWithType = ActivityType.Task,
        FollowUpDuration = FollowUpDuration.ThreeMonths,
        ActiveCurrencies = new[] { "USD", "EUR", "GBP", "AED" },
        DeactivatedCurrencies = new string[] { },
        LostReasons = new[] { "Price too high", "Lost to competitor", "No response", "Other" },
        AllowFreeFormReason = true,
    });

    // ToDo: Custom Activity Types
    public DateTimeOffset? DeactivatedAt { get; set; }

    // Navigation properties
    public IList<TenantUser> TenantUsers { get; private set; } = new List<TenantUser>();
    public IList<Subscription> Subscriptions { get; private set; } = new List<Subscription>();
}