using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityActivityConfiguration : BaseAuditableConfiguration<EntityActivity>
{
    public override void Configure(EntityTypeBuilder<EntityActivity> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(a => a.Type).IsRequired().HasConversion<int>();
        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.EntityType).IsRequired();

        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId }).HasDatabaseName("IX_Activity_TenantId_EntityType_EntityId");

        // Configure relationships
    }
}