using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class AssignmentRuleConditionConfiguration : BaseAuditableConfiguration<AssignmentRuleCondition>
{
    public override void Configure(EntityTypeBuilder<AssignmentRuleCondition> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(ar => ar.SortOrder).IsRequired();
        builder.Property(arc => arc.Field).IsRequired().HasConversion<string>();
        builder.Property(arc => arc.Operator).IsRequired().HasConversion<string>();
        builder.Property(arc => arc.Value).IsRequired().HasMaxLength(256);
        builder.Property(arc => arc.ValueTo).HasMaxLength(256);

        // Configure relationships
        builder.HasOne(arc => arc.AssignmentRulesSet).WithMany(ar => ar.Conditions).HasForeignKey(arc => arc.AssignmentRulesSetId).OnDelete(DeleteBehavior.Cascade);
    }
}