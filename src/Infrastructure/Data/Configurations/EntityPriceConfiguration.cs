using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityPriceConfiguration : BaseAuditableConfiguration<EntityPrice>
{
    public override void Configure(EntityTypeBuilder<EntityPrice> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(a => a.UnitPrice).IsRequired().HasColumnType("decimal(18,4)");
        builder.Property(a => a.CostPrice).HasColumnType("decimal(18,4)");
        builder.Property(a => a.DirectCost).HasColumnType("decimal(18,4)");
        builder.Property(a => a.Currency).IsRequired().HasMaxLength(3);
        builder.Property(a => a.Comment).HasMaxLength(1000);

        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.EntityType).IsRequired().HasConversion<string>();

        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId }).HasDatabaseName("IX_Price_TenantId_EntityType_EntityId");

        // Configure relationships
    }
}