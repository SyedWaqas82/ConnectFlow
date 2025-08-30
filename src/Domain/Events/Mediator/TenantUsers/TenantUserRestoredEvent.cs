namespace ConnectFlow.Domain.Events.Mediator.TenantUsers;

public class TenantUserRestoredEvent : BaseEvent
{
    public TenantUser TenantUser { get; }

    public TenantUserRestoredEvent(TenantUser tenantUser, int applicationUserId) : base(tenantUser.TenantId, applicationUserId)
    {
        TenantUser = tenantUser;
    }
}