// namespace ConnectFlow.Domain.Entities;

// public class Contact : BaseAuditableEntity, ITenantEntity, ISoftDelete
// {
//     public string FirstName { get; set; } = null!;
//     public string LastName { get; set; } = null!;
//     public string? Email { get; set; }
//     public string? Phone { get; set; }
//     public string? JobTitle { get; set; }

//     public int? CompanyId { get; set; }
//     public Company? Company { get; set; }

//     public int ScoreId { get; set; } = default!;
//     public ContactScore Score { get; set; } = null!;

//     public IList<Appointment> Appointments { get; private set; } = new List<Appointment>();
//     public IList<Lead> Leads { get; private set; } = new List<Lead>();

//     // Tenant
//     public int TenantId { get; set; } = default!;
//     public Tenant Tenant { get; set; } = null!;

//     // Soft delete properties
//     public bool IsDeleted { get; set; } = false;
//     public DateTimeOffset? DeletedAt { get; set; }
//     public int? DeletedBy { get; set; }
// }