namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

/// <summary>
/// Handles subscription status changes - suspension, reactivation, cancellation with limits management
/// </summary>
public class SubscriptionStatusEvent : BaseEvent
{
    public Subscription Subscription { get; }
    public SubscriptionAction Action { get; }
    public string Reason { get; }
    public bool SendEmailNotification { get; }
    public bool IsImmediate { get; }
    public int? PreviousPlanId { get; }
    public int? NewPlanId { get; }

    public SubscriptionStatusEvent(int tenantId, int applicationUserId, Subscription subscription, SubscriptionAction action, string reason, bool sendEmailNotification = true, bool isImmediate = true, int? previousPlanId = null, int? newPlanId = null) : base(tenantId, applicationUserId)
    {
        Subscription = subscription;
        Action = action;
        Reason = reason;
        SendEmailNotification = sendEmailNotification;
        IsImmediate = isImmediate;
        PreviousPlanId = previousPlanId;
        NewPlanId = newPlanId;
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