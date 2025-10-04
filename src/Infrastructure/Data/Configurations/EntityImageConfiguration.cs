using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityImageConfiguration : BaseAuditableConfiguration<EntityImage>
{
    public override void Configure(EntityTypeBuilder<EntityImage> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(a => a.ImageUrl).IsRequired().HasMaxLength(2048);
        builder.Property(a => a.AltText).HasMaxLength(512);
        builder.Property(a => a.DisplayOrder).IsRequired();

        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.EntityType).IsRequired().HasConversion<string>();

        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId }).HasDatabaseName("IX_Image_TenantId_EntityType_EntityId");

        // Configure relationships
    }
}