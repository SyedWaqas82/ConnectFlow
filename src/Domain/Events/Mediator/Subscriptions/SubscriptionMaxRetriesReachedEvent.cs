namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionMaxRetriesReachedEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public string PlanName { get; }
    public int TotalRetryAttempts { get; }
    public DateTimeOffset FirstFailureAt { get; }

    public SubscriptionMaxRetriesReachedEvent(int tenantId, int subscriptionId, string planName, int totalRetryAttempts, DateTimeOffset firstFailureAt)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        PlanName = planName;
        TotalRetryAttempts = totalRetryAttempts;
        FirstFailureAt = firstFailureAt;
    }
}
