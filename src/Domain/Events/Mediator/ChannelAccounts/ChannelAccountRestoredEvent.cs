namespace ConnectFlow.Domain.Events.Mediator.ChannelAccounts;

public class ChannelAccountRestoredEvent : BaseEvent
{
    public int ChannelAccountId { get; }

    public ChannelAccountRestoredEvent(int tenantId, int channelAccountId)
    {
        TenantId = tenantId;
        ChannelAccountId = channelAccountId;
    }
}