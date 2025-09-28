namespace ConnectFlow.Domain.Entities;

public class SchedulerSlot : BaseAuditableEntity, ITenantableEntity
{
    public int SchedulerId { get; set; }
    public Scheduler Scheduler { get; set; } = null!;
    public DateTimeOffset StartDateTime { get; set; }
    public DateTimeOffset EndDateTime { get; set; }
    public SchedulingSlotType SlotType { get; set; } = SchedulingSlotType.Available;
    public bool AllowMultipleBookings { get; set; } = false;
    public required string Title { get; set; } /// Title or name of the slot
    public string? Description { get; set; }

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}