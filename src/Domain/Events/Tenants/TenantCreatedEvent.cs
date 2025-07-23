namespace ConnectFlow.Domain.Events.Tenants;

public class TenantCreatedEvent : BaseEvent
{
    public Tenant Tenant { get; }

    public TenantCreatedEvent(Tenant tenant)
    {
        Tenant = tenant;
    }
}