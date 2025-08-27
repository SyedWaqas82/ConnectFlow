namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

/// <summary>
/// Handles subscription status changes - suspension, reactivation, cancellation with limits management
/// </summary>
public class SubscriptionStatusEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public SubscriptionAction Action { get; }
    public string Reason { get; }
    public bool SendEmailNotification { get; }
    public DateTimeOffset Timestamp { get; }
    public bool SuspendLimitsImmediately { get; }
    public int? PreviousPlanId { get; }
    public int? NewPlanId { get; }

    public SubscriptionStatusEvent(
        int subscriptionId,
        SubscriptionAction action,
        string reason,
        bool sendEmailNotification = true,
        bool suspendLimitsImmediately = false,
        int? previousPlanId = null,
        int? newPlanId = null,
        int? tenantId = null)
    {
        SubscriptionId = subscriptionId;
        Action = action;
        Reason = reason;
        SendEmailNotification = sendEmailNotification;
        SuspendLimitsImmediately = suspendLimitsImmediately;
        PreviousPlanId = previousPlanId;
        NewPlanId = newPlanId;
        Timestamp = DateTimeOffset.UtcNow;
        TenantId = tenantId;
    }
}

public enum SubscriptionAction
{
    Create,
    Suspend,
    Reactivate,
    Cancel,
    GracePeriodStart,
    GracePeriodEnd,
    PlanChanged,
    StatusUpdate
}