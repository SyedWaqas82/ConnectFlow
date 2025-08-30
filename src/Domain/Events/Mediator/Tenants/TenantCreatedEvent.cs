namespace ConnectFlow.Domain.Events.Mediator.Tenants;

public class TenantCreatedEvent : BaseEvent
{
    public Tenant Tenant { get; }

    public TenantCreatedEvent(Tenant tenant, int applicationUserId) : base(tenant.Id, applicationUserId)
    {
        Tenant = tenant;
    }
}