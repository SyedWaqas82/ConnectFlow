namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class SubscriptionCancelledEvent : BaseEvent
{
    public Subscription Subscription { get; }
    public CancellationType CancellationType { get; }
    public DateTime? EffectiveDate { get; }
    public int UsersToSuspend { get; }
    public int ChannelAccountsToSuspend { get; }

    public SubscriptionCancelledEvent(Subscription subscription, CancellationType cancellationType, DateTime? effectiveDate, int usersToSuspend, int channelAccountsToSuspend)
    {
        Subscription = subscription;
        CancellationType = cancellationType;
        EffectiveDate = effectiveDate;
        UsersToSuspend = usersToSuspend;
        ChannelAccountsToSuspend = channelAccountsToSuspend;
    }
}