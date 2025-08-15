// namespace ConnectFlow.Domain.Entities;

// public class Pipeline : BaseAuditableEntity, ITenantEntity, ISoftDelete
// {
//     public string Name { get; set; } = string.Empty;
//     public string Description { get; set; } = string.Empty;
//     public bool IsDefault { get; set; }
//     public bool IsActive { get; set; } = true;

//     public IList<Stage> Stages { get; private set; } = new List<Stage>();
//     public IList<Lead> Leads { get; private set; } = new List<Lead>();

//     // Tenant
//     public int TenantId { get; set; } = default!;
//     public Tenant Tenant { get; set; } = null!;

//     // Soft delete properties
//     public bool IsDeleted { get; set; } = false;
//     public DateTimeOffset? DeletedAt { get; set; }
//     public int? DeletedBy { get; set; }
// }
