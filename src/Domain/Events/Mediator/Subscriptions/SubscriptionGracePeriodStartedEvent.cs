namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionGracePeriodStartedEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public string PlanName { get; }
    public DateTimeOffset GracePeriodEndsAt { get; }

    public SubscriptionGracePeriodStartedEvent(int tenantId, int subscriptionId, string planName, DateTimeOffset gracePeriodEndsAt)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        PlanName = planName;
        GracePeriodEndsAt = gracePeriodEndsAt;
    }
}
