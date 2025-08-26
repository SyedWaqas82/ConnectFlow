namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionStatusUpdatedEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public string PlanName { get; }
    public SubscriptionStatus PreviousStatus { get; }
    public SubscriptionStatus NewStatus { get; }

    public SubscriptionStatusUpdatedEvent(int tenantId, int subscriptionId, string planName, SubscriptionStatus previousStatus, SubscriptionStatus newStatus)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        PlanName = planName;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
    }
}
