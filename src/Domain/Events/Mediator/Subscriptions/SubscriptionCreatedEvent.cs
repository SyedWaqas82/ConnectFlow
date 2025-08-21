namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionCreatedEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public string PlanName { get; }

    public SubscriptionCreatedEvent(int tenantId, int subscriptionId, string planName)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        PlanName = planName;
    }
}