using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ScoringRuleConditionConfiguration : BaseAuditableConfiguration<ScoringRuleCondition>
{
    public override void Configure(EntityTypeBuilder<ScoringRuleCondition> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(src => src.ConditionType).IsRequired().HasConversion<string>();
        builder.Property(src => src.FieldName).IsRequired().HasMaxLength(200);
        builder.Property(src => src.Operator).IsRequired().HasConversion<string>();
        builder.Property(src => src.ComparisonValue).IsRequired().HasMaxLength(1000);
        builder.Property(src => src.LogicalOperator).IsRequired().HasConversion<string>();
        builder.Property(src => src.SortOrder).IsRequired();

        // Configure relationships
        builder.HasOne(src => src.ScoringCriteria).WithMany(sc => sc.Conditions).HasForeignKey(src => src.ScoringCriteriaId).OnDelete(DeleteBehavior.Cascade);
    }
}