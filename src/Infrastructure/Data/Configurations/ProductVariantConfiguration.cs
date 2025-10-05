using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ProductVariantConfiguration : BaseAuditableConfiguration<ProductVariant>
{
    public override void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        base.Configure(builder);

        builder.Property(pv => pv.Name).IsRequired().HasMaxLength(100);

        // Configure relationships
        builder.HasOne(pv => pv.Product).WithMany(p => p.Variants).HasForeignKey(pv => pv.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(pv => pv.DealProducts).WithOne(dp => dp.ProductVariant).HasForeignKey(dp => dp.ProductVariantId).OnDelete(DeleteBehavior.SetNull);
    }
}