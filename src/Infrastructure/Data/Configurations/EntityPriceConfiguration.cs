using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityPriceConfiguration : BaseAuditableConfiguration<EntityPrice>
{
    public override void Configure(EntityTypeBuilder<EntityPrice> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.EntityType).IsRequired();

        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId }).HasDatabaseName("IX_Price_TenantId_EntityType_EntityId");

        // Configure relationships
    }
}