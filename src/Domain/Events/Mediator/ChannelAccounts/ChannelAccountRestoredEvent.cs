namespace ConnectFlow.Domain.Events.Mediator.ChannelAccounts;

public class ChannelAccountRestoredEvent : BaseEvent
{
    public int ChannelAccountId { get; }

    public ChannelAccountRestoredEvent(int tenantId, int applicationUserId, int channelAccountId) : base(tenantId, applicationUserId)
    {
        ChannelAccountId = channelAccountId;
    }
}