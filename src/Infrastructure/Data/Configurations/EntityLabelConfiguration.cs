using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityLabelConfiguration : BaseAuditableConfiguration<EntityLabel>
{
    public override void Configure(EntityTypeBuilder<EntityLabel> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.EntityType).IsRequired().HasConversion<string>();

        builder.HasIndex(a => new { a.EntityType, a.EntityId }).HasDatabaseName("IX_EntityLabel_EntityType_EntityId");

        // Configure relationships
        builder.HasOne(a => a.Label).WithMany(l => l.Labels).HasForeignKey(a => a.LabelId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.AssignedByUser).WithMany(tu => tu.AssignedByLabels).HasForeignKey(a => a.AssignedBy).OnDelete(DeleteBehavior.Restrict);
    }
}