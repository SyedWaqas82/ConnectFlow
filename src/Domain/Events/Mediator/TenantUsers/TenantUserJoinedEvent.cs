namespace ConnectFlow.Domain.Events.Mediator.TenantUsers;

public class TenantUserJoinedEvent : BaseEvent
{
    public TenantUser TenantUser { get; }

    public TenantUserJoinedEvent(TenantUser tenantUser, int applicationUserId) : base(tenantUser.TenantId, applicationUserId)
    {
        TenantUser = tenantUser;
    }
}