// namespace ConnectFlow.Domain.Entities;

// public class Resource : BaseEntity, ITenantEntity
// {
//     public string Name { get; set; } = null!;
//     public string? Description { get; set; }
//     public ResourceType Type { get; set; }
//     public bool IsAvailable { get; set; }

//     public IList<Service> Services { get; private set; } = new List<Service>();
//     public IList<Appointment> Appointments { get; private set; } = new List<Appointment>();

//     // Tenant
//     public int TenantId { get; set; } = default!;
//     public Tenant Tenant { get; set; } = null!;
// }