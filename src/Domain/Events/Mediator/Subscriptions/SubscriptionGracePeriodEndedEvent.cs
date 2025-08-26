namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionGracePeriodEndedEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public string PlanName { get; }
    public bool WasAutoDowngraded { get; }

    public SubscriptionGracePeriodEndedEvent(int tenantId, int subscriptionId, string planName, bool wasAutoDowngraded)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        PlanName = planName;
        WasAutoDowngraded = wasAutoDowngraded;
    }
}
