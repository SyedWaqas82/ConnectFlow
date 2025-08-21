namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionReactivatedEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public string PlanName { get; }

    public SubscriptionReactivatedEvent(int tenantId, int subscriptionId, string planName)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        PlanName = planName;
    }
}