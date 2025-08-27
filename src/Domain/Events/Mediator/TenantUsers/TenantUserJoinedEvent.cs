namespace ConnectFlow.Domain.Events.Mediator.TenantUsers;

public class TenantUserJoinedEvent : BaseEvent
{

    public TenantUserJoinedEvent(int tenantId, int applicationUserId) : base(tenantId, applicationUserId)
    {
    }
}