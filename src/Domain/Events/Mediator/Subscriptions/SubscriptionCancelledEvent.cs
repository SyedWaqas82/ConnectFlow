namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionCancelledEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public bool ImmediateCancellation { get; }

    public SubscriptionCancelledEvent(int tenantId, int subscriptionId, bool immediateCancellation)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;

    }
}