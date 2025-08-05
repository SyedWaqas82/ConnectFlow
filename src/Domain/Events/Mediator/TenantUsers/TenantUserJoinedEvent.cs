namespace ConnectFlow.Domain.Events.Mediator.TenantUsers;

public class TenantUserJoinedEvent : BaseEvent
{
    public TenantUser TenantUser { get; }

    public TenantUserJoinedEvent(TenantUser tenantUser)
    {
        TenantUser = tenantUser;
    }
}