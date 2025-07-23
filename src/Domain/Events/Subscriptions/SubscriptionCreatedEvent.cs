namespace ConnectFlow.Domain.Events.Subscriptions;

public class SubscriptionCreatedEvent : BaseEvent
{
    public Subscription Subscription { get; }

    public SubscriptionCreatedEvent(Subscription subscription)
    {
        Subscription = subscription;
    }
}