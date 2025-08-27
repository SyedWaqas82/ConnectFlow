namespace ConnectFlow.Domain.Events.Mediator.Tenants;

public class TenantCreatedEvent : BaseEvent
{
    public TenantCreatedEvent(int tenantId, int applicationUserId) : base(tenantId, applicationUserId)
    {
    }
}