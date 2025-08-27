namespace ConnectFlow.Domain.Events.Mediator.ChannelAccounts;

public class ChannelAccountSuspendedEvent : BaseEvent
{
    public int ChannelAccountId { get; }

    public ChannelAccountSuspendedEvent(int tenantId, int channelAccountId)
    {
        TenantId = tenantId;
        ChannelAccountId = channelAccountId;
    }
}