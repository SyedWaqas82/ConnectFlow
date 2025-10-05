using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ProductConfiguration : BaseAuditableConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Code).HasMaxLength(50);
        builder.Property(p => p.Unit).HasMaxLength(20);
        builder.Property(p => p.TaxPercentage).HasColumnType("decimal(5,2)");
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.BillingFrequency).IsRequired().HasConversion<string>();

        // Configure relationships
        builder.HasOne(p => p.Owner).WithMany(tu => tu.Products).HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Category).WithMany(pc => pc.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(p => p.DealProducts).WithOne(dp => dp.Product).HasForeignKey(dp => dp.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Variants).WithOne(pv => pv.Product).HasForeignKey(pv => pv.ProductId).OnDelete(DeleteBehavior.Cascade);

        // Configure Indexes

        // Foreign key indexes
        builder.HasIndex(p => p.OwnerId).HasDatabaseName("IX_Product_OwnerId");

        builder.HasIndex(p => p.CategoryId).HasDatabaseName("IX_Product_CategoryId");

        // Common search indexes
        builder.HasIndex(p => p.Name).HasDatabaseName("IX_Product_Name");
        builder.HasIndex(p => p.Code).HasDatabaseName("IX_Product_Code");

        // Filter by billing frequency (common query pattern)
        builder.HasIndex(p => p.BillingFrequency).HasDatabaseName("IX_Product_BillingFrequency");
    }
}