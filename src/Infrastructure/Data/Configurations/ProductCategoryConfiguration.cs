using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ProductCategoryConfiguration : BaseAuditableConfiguration<ProductCategory>
{
    public override void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        base.Configure(builder);

        builder.Property(pc => pc.Name).IsRequired().HasMaxLength(100);
        builder.Property(pc => pc.SortOrder).IsRequired();
        builder.Property(pc => pc.Description).HasMaxLength(500);

        // Configure relationships
        builder.HasOne(pc => pc.ParentCategory).WithMany(pc => pc.ChildCategories).HasForeignKey(pc => pc.ParentCategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(pc => pc.Products).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.SetNull);

        // Configure Indexes

        // Foreign key index
        builder.HasIndex(pc => pc.ParentCategoryId).HasDatabaseName("IX_ProductCategory_ParentCategoryId");

        // Name index for searches
        builder.HasIndex(pc => pc.Name).HasDatabaseName("IX_ProductCategory_Name");

        // Sort order index for ordering
        builder.HasIndex(pc => pc.SortOrder).HasDatabaseName("IX_ProductCategory_SortOrder");
    }
}