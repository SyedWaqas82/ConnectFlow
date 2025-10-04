namespace ConnectFlow.Domain.Entities;

public class DealProduct : BaseAuditableEntity, ITenantableEntity
{
    public int DealId { get; set; }
    public Deal Deal { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    public int SortOrder { get; set; }
    public DateTimeOffset? BillingStartDate { get; set; }
    public DiscountType DiscountType { get; set; } = DiscountType.FixedAmount;
    public decimal? DiscountValue { get; set; }
    public decimal? TaxPercentage { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Notes { get; set; }
    public string? AdditionalDiscount { get; set; } // jsonb for additional discounts

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}