using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class AssignmentRulesSetConfiguration : BaseAuditableConfiguration<AssignmentRulesSet>
{
    public override void Configure(EntityTypeBuilder<AssignmentRulesSet> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(ars => ars.SortOrder).IsRequired();
        builder.Property(ars => ars.LogicalOperator).IsRequired().HasConversion<string>();

        // Configure relationships
        builder.HasOne(ars => ars.AssignmentRule).WithMany(ar => ar.RulesSets).HasForeignKey(ars => ars.AssignmentRuleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(ars => ars.Conditions).WithOne(arc => arc.AssignmentRulesSet).HasForeignKey(arc => arc.AssignmentRulesSetId).OnDelete(DeleteBehavior.Cascade);
    }
}