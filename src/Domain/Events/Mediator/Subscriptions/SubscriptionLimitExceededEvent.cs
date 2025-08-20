namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionLimitExceededEvent : BaseEvent
{
    public Subscription Subscription { get; }
    public LimitValidationType LimitType { get; }
    public int CurrentUsage { get; }
    public int AllowedLimit { get; }
    public SubscriptionPlan? RecommendedUpgrade { get; }

    public SubscriptionLimitExceededEvent(Subscription subscription, LimitValidationType limitType, int currentUsage, int allowedLimit, SubscriptionPlan? recommendedUpgrade)
    {
        Subscription = subscription;
        LimitType = limitType;
        CurrentUsage = currentUsage;
        AllowedLimit = allowedLimit;
        RecommendedUpgrade = recommendedUpgrade;
    }
}