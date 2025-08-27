namespace ConnectFlow.Domain.Events.Mediator.TenantUsers;

public class TenantUserRestoredEvent : BaseEvent
{
    public int TenantUserId { get; }

    public TenantUserRestoredEvent(int tenantId, int applicationUserId, int tenantUserId) : base(tenantId, applicationUserId)
    {
        TenantUserId = tenantUserId;
    }
}