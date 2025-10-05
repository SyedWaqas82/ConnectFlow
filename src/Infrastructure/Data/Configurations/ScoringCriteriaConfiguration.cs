using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ScoringCriteriaConfiguration : BaseAuditableConfiguration<ScoringCriteria>
{
    public override void Configure(EntityTypeBuilder<ScoringCriteria> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(sc => sc.Name).IsRequired().HasMaxLength(200);
        builder.Property(sc => sc.Description).HasMaxLength(2000);
        builder.Property(sc => sc.SortOrder).IsRequired();
        builder.Property(sc => sc.LogicalOperator).IsRequired().HasConversion<string>();

        // Configure relationships
        builder.HasOne(sc => sc.ScoringGroup).WithMany(sg => sg.ScoringCriterias).HasForeignKey(sc => sc.ScoringGroupId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(sc => sc.Conditions).WithOne(c => c.ScoringCriteria).HasForeignKey(c => c.ScoringCriteriaId).OnDelete(DeleteBehavior.Cascade);
    }
}