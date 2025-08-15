// namespace ConnectFlow.Domain.Entities;

// public class Company : BaseAuditableEntity, ITenantEntity, ISoftDelete
// {
//     public string Name { get; set; } = null!;
//     public string? Website { get; set; }
//     public string? Industry { get; set; }

//     public IList<Contact> Contacts { get; private set; } = new List<Contact>();
//     public IList<Lead> Leads { get; private set; } = new List<Lead>();

//     // Tenant
//     public int TenantId { get; set; } = default!;
//     public Tenant Tenant { get; set; } = null!;

//     // Soft delete properties
//     public bool IsDeleted { get; set; } = false;
//     public DateTimeOffset? DeletedAt { get; set; }
//     public int? DeletedBy { get; set; }
// }