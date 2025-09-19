namespace ConnectFlow.Domain.Entities;

public class ProductCategory : BaseAuditableEntity, ITenantableEntity
{
    public required string Name { get; set; }
    public int SortOrder { get; set; }
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public ProductCategory? ParentCategory { get; set; }
    public IList<ProductCategory> ChildCategories { get; private set; } = new List<ProductCategory>();
    public IList<Product> Products { get; private set; } = new List<Product>();

    // TenentableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}