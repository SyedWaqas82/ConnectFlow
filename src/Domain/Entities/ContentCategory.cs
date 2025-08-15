// namespace ConnectFlow.Domain.Entities;

// public class ContentCategory : BaseAuditableEntity, ITenantEntity
// {
//     public string Name { get; set; } = string.Empty;
//     public string Description { get; set; } = string.Empty;
//     public string Slug { get; set; } = string.Empty;
//     public int Order { get; set; }
//     public int? ParentId { get; set; }
//     public ContentCategory? Parent { get; set; }

//     public IList<ContentCategory> Children { get; private set; } = new List<ContentCategory>();

//     // Tenant
//     public int TenantId { get; set; } = default!;
//     public Tenant Tenant { get; set; } = null!;
// }
