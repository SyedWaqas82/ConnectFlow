namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionDowngradedEvent : BaseEvent
{
    public Subscription FromSubscription { get; }
    public Subscription ToSubscription { get; }
    public int SuspendedUsersCount { get; }
    public int SuspendedChannelAccountsCount { get; }

    public SubscriptionDowngradedEvent(Subscription fromSubscription, Subscription toSubscription,
        int suspendedUsersCount, int suspendedChannelAccountsCount)
    {
        FromSubscription = fromSubscription;
        ToSubscription = toSubscription;
        SuspendedUsersCount = suspendedUsersCount;
        SuspendedChannelAccountsCount = suspendedChannelAccountsCount;
    }
}