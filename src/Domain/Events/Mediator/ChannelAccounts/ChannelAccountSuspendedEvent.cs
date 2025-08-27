namespace ConnectFlow.Domain.Events.Mediator.ChannelAccounts;

public class ChannelAccountSuspendedEvent : BaseEvent
{
    public int ChannelAccountId { get; }

    public ChannelAccountSuspendedEvent(int tenantId, int applicationUserId, int channelAccountId) : base(tenantId, applicationUserId)
    {
        ChannelAccountId = channelAccountId;
    }
}