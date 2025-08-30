namespace ConnectFlow.Domain.Events.Mediator.TenantUsers;

public class TenantUserSuspendedEvent : BaseEvent
{
    public TenantUser TenantUser { get; }

    public TenantUserSuspendedEvent(TenantUser tenantUser, int applicationUserId) : base(tenantUser.TenantId, applicationUserId)
    {
        TenantUser = tenantUser;
    }
}