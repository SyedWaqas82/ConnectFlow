namespace ConnectFlow.Domain.Events.Mediator.ChannelAccounts;

public class ChannelAccountSuspendedEvent : BaseEvent
{
    public ChannelAccount ChannelAccount { get; }

    public ChannelAccountSuspendedEvent(int tenantId, int applicationUserId, ChannelAccount channelAccount) : base(tenantId, applicationUserId)
    {
        ChannelAccount = channelAccount;
    }
}