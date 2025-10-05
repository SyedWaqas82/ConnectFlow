namespace ConnectFlow.Domain.Entities;

public class SchedulerSlot : BaseAuditableEntity, ITenantableEntity
{
    public required string Title { get; set; } /// Title or name of the slot
    public string? Description { get; set; }
    public SchedulingSlotType SlotType { get; set; } = SchedulingSlotType.Available;
    public DateTimeOffset StartDateTime { get; set; }
    public DateTimeOffset EndDateTime { get; set; }
    public bool AllowMultipleBookings { get; set; } = false;
    public int? SchedulerId { get; set; }
    public Scheduler Scheduler { get; set; } = null!; // Navigation property to Manual Scheduler
    public int? AvailabilityId { get; set; } // If this slot was generated from an availability
    public SchedulerAvailability Availability { get; set; } = null!; // Navigation property to the availability it was generated from
    public IList<SchedulerBooking> Bookings { get; private set; } = new List<SchedulerBooking>(); // Navigation property for related bookings

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}