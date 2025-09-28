namespace ConnectFlow.Domain.Entities;

public class SchedulerAvailability : BaseAuditableEntity, ITenantableEntity
{
    public int SchedulerId { get; set; }
    public Scheduler Scheduler { get; set; } = null!;
    public ScheduleDayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly? SpecificDate { get; set; } // If set, this availability is for a specific date (overrides DayOfWeek)
    public bool AllowMultipleBookings { get; set; } = false; // If true, multiple bookings allowed in this slot

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}