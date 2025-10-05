namespace ConnectFlow.Domain.Entities;

public class Scheduler : BaseAuditableEntity, ITenantableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int OwnerId { get; set; }
    public TenantUser Owner { get; set; } = null!;
    public int DefaultDurationMinutes { get; set; } = 30; // Default meeting duration
    public DateTimeOffset AvailableFrom { get; set; } // When the scheduler becomes active
    public DateTimeOffset? AvailableTo { get; set; } // When the scheduler is no longer active
    public int BeforeEventBufferTimeMinutes { get; set; } = 0; // Buffer time before each event
    public int AfterEventBufferTimeMinutes { get; set; } = 0; // Buffer time after each event
    public StartingInterval StartingIntervalMinutes { get; set; } = StartingInterval.OnTheFifteenMinutes; // Invitees able to choose start times in these increments
    public MinimumNoticeToBook MinimumNoticeToBook { get; set; } = MinimumNoticeToBook.NoMinimum; // Minimum notice required to book
    public MaximumAdvanceToBook FurthestNoticeToBookInFutureDays { get; set; } = MaximumAdvanceToBook.SixMonths; // How far in advance can be booked (in minutes)
    public string Timezone { get; set; } = "UTC";
    public required string DefaultSubject { get; set; }
    public ActivityType Type { get; set; }
    public string MeetingNote { get; set; } = string.Empty; // attach to each scheduled meeting
    public string DefaultLocation { get; set; } = string.Empty;
    public string DefaultConferenceUrl { get; set; } = string.Empty;
    public string MeetingDescription { get; set; } = string.Empty; // Display on calendar details
    public string VisibleCompanyInfo { get; set; } = string.Empty; // Info about the company shown on booking page
    public bool IsPhoneNumberRequired { get; set; } = false; // Whether booker phone number is required
    public string? CustomBookingFormFields { get; set; } // JSON array of custom form fields to show on booking page
    public string FooterNote { get; set; } = string.Empty; // Note shown at bottom of booking page
    public string? UrlSlug { get; set; } // Custom URL slug for the scheduler
    public bool ManageAvailabilityManually { get; set; } = false; // If true, only manual slots can be booked (no recurring availability)

    // Navigation properties
    public IList<SchedulerAvailability> Availabilities { get; private set; } = new List<SchedulerAvailability>();
    public IList<SchedulerSlot> ManualSlots { get; private set; } = new List<SchedulerSlot>();

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}