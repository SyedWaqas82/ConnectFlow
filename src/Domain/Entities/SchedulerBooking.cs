namespace ConnectFlow.Domain.Entities;

public class SchedulerBooking : BaseAuditableEntity, ITenantableEntity
{
    public int SchedulerId { get; set; }
    public Scheduler Scheduler { get; set; } = null!;
    public int ActivityId { get; set; }
    public EntityActivity Activity { get; set; } = null!;
    public int? SchedulerSlotId { get; set; }
    public SchedulerSlot? SchedulerSlot { get; set; }
    public string? BookerEmail { get; set; }
    public string? BookerName { get; set; }
    public string? BookerPhone { get; set; }
    public string? BookingFormData { get; set; } // JSON or serialized form data
    public string? BookingSource { get; set; } // "Website", "API", "Manual", etc.
    public bool ReminderSent { get; set; } = false;
    public DateTimeOffset? ReminderSentAt { get; set; }
    public bool IsCancelled { get; set; } = false;
    public DateTimeOffset? CancelledAt { get; set; }
    public int? CancelledById { get; set; }
    public TenantUser? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }
    public int? RescheduledFromBookingId { get; set; }
    public SchedulerBooking? RescheduledFromBooking { get; set; }
    public int? RescheduledToBookingId { get; set; }
    public SchedulerBooking? RescheduledToBooking { get; set; }
    public IList<EntityActivity> Activities { get; private set; } = new List<EntityActivity>();

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}