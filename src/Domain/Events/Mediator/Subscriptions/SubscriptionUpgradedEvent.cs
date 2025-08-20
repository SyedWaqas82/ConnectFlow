namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionUpgradedEvent : BaseEvent
{
    public Subscription FromSubscription { get; }
    public Subscription ToSubscription { get; }
    public int ReactivatedUsersCount { get; }
    public int ReactivatedChannelAccountsCount { get; }

    public SubscriptionUpgradedEvent(Subscription fromSubscription, Subscription toSubscription, int reactivatedUsersCount, int reactivatedChannelAccountsCount)
    {
        FromSubscription = fromSubscription;
        ToSubscription = toSubscription;
        ReactivatedUsersCount = reactivatedUsersCount;
        ReactivatedChannelAccountsCount = reactivatedChannelAccountsCount;
    }
}