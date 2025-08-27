namespace ConnectFlow.Domain.Events.Mediator.TenantUsers;

public class TenantUserSuspendedEvent : BaseEvent
{
    public int TenantUserId { get; }

    public TenantUserSuspendedEvent(int tenantId, int applicationUserId, int tenantUserId) : base(tenantId, applicationUserId)
    {
        TenantUserId = tenantUserId;
    }
}