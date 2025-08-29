namespace ConnectFlow.Domain.Events.Messaging;

public class SubscriptionMessageEvent : BaseMessageEvent
{
    public SubscriptionMessageEvent(int tenantId, int applicationUserId) : base(tenantId, applicationUserId)
    {
    }

    public override string MessageType => nameof(SubscriptionMessageEvent);
}