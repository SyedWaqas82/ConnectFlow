namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionPastDueEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public string PlanName { get; }
    public DateTimeOffset PastDueDate { get; }

    public SubscriptionPastDueEvent(int tenantId, int subscriptionId, string planName, DateTimeOffset pastDueDate)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        PlanName = planName;
        PastDueDate = pastDueDate;
    }
}