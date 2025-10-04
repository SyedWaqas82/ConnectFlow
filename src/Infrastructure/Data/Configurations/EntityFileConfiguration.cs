using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityFileConfiguration : BaseAuditableConfiguration<EntityFile>
{
    public override void Configure(EntityTypeBuilder<EntityFile> builder)
    {
        base.Configure(builder);

        //Configure properties
        builder.Property(a => a.FileName).IsRequired().HasMaxLength(255);
        builder.Property(a => a.FileType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.FileSize).IsRequired();
        builder.Property(a => a.Url).IsRequired().HasMaxLength(2048);

        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.EntityType).IsRequired().HasConversion<string>();

        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId }).HasDatabaseName("IX_File_TenantId_EntityType_EntityId");

        // Configure relationships
    }
}