using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class AssignmentRuleConfiguration : BaseAuditableConfiguration<AssignmentRule>
{
    public override void Configure(EntityTypeBuilder<AssignmentRule> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(ar => ar.Name).IsRequired().HasMaxLength(128);
        builder.Property(ar => ar.Description).HasMaxLength(256);
        builder.Property(ar => ar.EntityType).IsRequired().HasConversion<string>();
        builder.Property(ar => ar.TriggerEvent).IsRequired().HasConversion<string>();
        builder.Property(ar => ar.AssignToUserId).IsRequired();
        builder.Property(ar => ar.SortOrder).IsRequired();
        builder.Property(ar => ar.IsActive).IsRequired();

        // Configure relationships
        builder.HasOne(ar => ar.AssignToUser).WithMany(tu => tu.AssignmentRules).HasForeignKey(ar => ar.AssignToUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(ar => ar.RulesSets).WithOne(ars => ars.AssignmentRule).HasForeignKey(ars => ars.AssignmentRuleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(ar => ar.AssignmentHistories).WithOne(ah => ah.AssignmentRule).HasForeignKey(ah => ah.AssignmentRuleId).OnDelete(DeleteBehavior.Cascade);
    }
}