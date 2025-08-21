namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionUpgradedEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public string FromPlan { get; }
    public string ToPlan { get; }

    public SubscriptionUpgradedEvent(int tenantId, int subscriptionId, string fromPlan, string toPlan)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        FromPlan = fromPlan;
        ToPlan = toPlan;
    }
}