namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionDowngradedEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public string FromPlan { get; }
    public string ToPlan { get; }

    public SubscriptionDowngradedEvent(int tenantId, int subscriptionId, string fromPlan, string toPlan)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        FromPlan = fromPlan;
        ToPlan = toPlan;
    }
}