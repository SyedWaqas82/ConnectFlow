namespace ConnectFlow.Domain.Events.Mediator.ChannelAccounts;

public class ChannelAccountRestoredEvent : BaseEvent
{
    public ChannelAccount ChannelAccount { get; }

    public ChannelAccountRestoredEvent(int tenantId, int applicationUserId, ChannelAccount channelAccount) : base(tenantId, applicationUserId)
    {
        ChannelAccount = channelAccount;
    }
}