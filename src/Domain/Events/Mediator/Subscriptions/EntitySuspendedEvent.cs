namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class EntitySuspendedEvent : BaseEvent
{
    public string Reason { get; }

    public EntitySuspendedEvent(string reason)
    {
        Reason = reason;
    }
}