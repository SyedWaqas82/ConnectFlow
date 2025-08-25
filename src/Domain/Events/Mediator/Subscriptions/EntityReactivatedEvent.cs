namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class EntityReactivatedEvent : BaseEvent
{
    public string Reason { get; }

    public EntityReactivatedEvent(string reason)
    {
        Reason = reason;
    }
}