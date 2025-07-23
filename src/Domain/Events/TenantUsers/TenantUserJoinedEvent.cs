namespace ConnectFlow.Domain.Events.TenantUsers;

public class TenantUserJoinedEvent : BaseEvent
{
    public TenantUser TenantUser { get; }

    public TenantUserJoinedEvent(TenantUser tenantUser)
    {
        TenantUser = tenantUser;
    }
}