namespace ConnectFlow.Domain.Events;

public class TenantUserCreatedEvent : BaseEvent
{
    public TenantUser TenantUser { get; }

    public TenantUserCreatedEvent(TenantUser tenantUser)
    {
        TenantUser = tenantUser;
    }
}