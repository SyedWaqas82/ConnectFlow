using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Domain.Entities;

public class Product : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity, IFileableEntity, IPriceableEntity, IImageableEntity
{
    public required string Name { get; set; }
    public string? Code { get; set; }
    public string? Unit { get; set; }
    public decimal? TaxPercentage { get; set; }
    public int? CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
    public string? Description { get; set; }
    public BillingFrequency? BillingFrequency { get; set; }
    public bool RenewUntilCancelled { get; set; } = false;
    public int? RecurringCycleCount { get; set; }
    public IList<ProductParticipant> Participants { get; private set; } = new List<ProductParticipant>();
    public IList<DealProduct> DealProducts { get; set; } = new List<DealProduct>();
    public IList<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    [NotMapped]
    public EntityType EntityType => EntityType.Product;
    [NotMapped]
    public IList<EntityFile> Files { get; set; } = new List<EntityFile>();
    [NotMapped]
    public IList<EntityPrice> Prices { get; set; } = new List<EntityPrice>();
    [NotMapped]
    public IList<EntityImage> Images { get; set; } = new List<EntityImage>();

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ISoftDeleteableEntity implementation
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }

    // ISuspendibleEntity implementation
    public EntityStatus EntityStatus { get; set; } = EntityStatus.Active;
    public DateTimeOffset? SuspendedAt { get; set; }
    public DateTimeOffset? ResumedAt { get; set; }
}