using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Domain.Entities;

public class ProductVariant : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, IPriceableEntity
{
    public required string Name { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ISoftDeleteableEntity implementation
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }

    [NotMapped]
    public EntityType EntityType => EntityType.ProductVariant;

    [NotMapped]
    public IList<EntityPrice> Prices { get; set; } = new List<EntityPrice>();
}