// namespace ConnectFlow.Domain.Entities;

// public class Appointment : BaseEntity, ITenantEntity
// {
//     public DateTimeOffset StartTime { get; set; }
//     public DateTimeOffset EndTime { get; set; }
//     public string Title { get; set; } = null!;
//     public string? Description { get; set; }
//     public AppointmentStatus Status { get; set; }

//     public int? ContactId { get; set; }
//     public Contact? Contact { get; set; }

//     public int? ServiceId { get; set; }
//     public Service? Service { get; set; }

//     public IList<Resource> Resources { get; private set; } = new List<Resource>();

//     // Tenant
//     public int TenantId { get; set; } = default!;
//     public Tenant Tenant { get; set; } = null!;
// }
