using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class DealProductConfiguration : BaseAuditableConfiguration<DealProduct>
{
    public override void Configure(EntityTypeBuilder<DealProduct> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(dp => dp.SortOrder).IsRequired();
        builder.Property(dp => dp.DiscountType).IsRequired().HasConversion<string>();
        builder.Property(dp => dp.DiscountValue).HasColumnType("decimal(18,2)");
        builder.Property(dp => dp.TaxPercentage).HasColumnType("decimal(5,2)");
        builder.Property(dp => dp.Quantity).IsRequired();
        builder.Property(dp => dp.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(dp => dp.Currency).IsRequired().HasMaxLength(3);
        builder.Property(dp => dp.Notes).HasMaxLength(1000);
        builder.Property(dp => dp.AdditionalDiscount).HasColumnType("jsonb");

        // Add indexes for foreign keys to improve join performance
        builder.HasIndex(dp => dp.DealId).HasDatabaseName("IX_DealProduct_DealId");
        builder.HasIndex(dp => dp.ProductId).HasDatabaseName("IX_DealProduct_ProductId");
        builder.HasIndex(dp => dp.ProductVariantId).HasDatabaseName("IX_DealProduct_ProductVariantId");

        // Add composite index for product filtering within a tenant
        builder.HasIndex(dp => new { dp.TenantId, dp.ProductId }).HasDatabaseName("IX_DealProduct_TenantId_ProductId");

        // Configure relationships
        builder.HasOne(dp => dp.Deal).WithMany(d => d.DealProducts).HasForeignKey(dp => dp.DealId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(dp => dp.Product).WithMany(p => p.DealProducts).HasForeignKey(dp => dp.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(dp => dp.ProductVariant).WithMany(pv => pv.DealProducts).HasForeignKey(dp => dp.ProductVariantId).OnDelete(DeleteBehavior.SetNull);
    }
}