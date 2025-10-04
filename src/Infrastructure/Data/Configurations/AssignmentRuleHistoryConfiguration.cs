using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class AssignmentRuleHistoryConfiguration : BaseAuditableConfiguration<AssignmentRuleHistory>
{
    public override void Configure(EntityTypeBuilder<AssignmentRuleHistory> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(ah => ah.EntityType).IsRequired().HasConversion<string>();
        builder.Property(ah => ah.EntityId).IsRequired();
        builder.Property(ah => ah.EntityTitle).IsRequired().HasMaxLength(256);
        builder.Property(ah => ah.ExecutionResult).IsRequired().HasConversion<string>();
        builder.Property(ah => ah.ExecutionDate).IsRequired();
        builder.Property(ah => ah.ExecutionTimeMs).IsRequired();
        builder.Property(ah => ah.ErrorMessage).HasMaxLength(1000);
        builder.Property(ah => ah.TriggerEventSource).IsRequired().HasConversion<string>();

        // Configure relationships
        builder.HasOne(ah => ah.AssignmentRule).WithMany(ar => ar.AssignmentHistories).HasForeignKey(ah => ah.AssignmentRuleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ah => ah.PreviousAssignedUser).WithMany(tu => tu.PreviousAssignmentRuleHistories).HasForeignKey(ah => ah.PreviousAssignedUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(ah => ah.NewAssignedUser).WithMany(tu => tu.NewAssignmentRuleHistories).HasForeignKey(ah => ah.NewAssignedUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(ah => ah.TriggeredByUser).WithMany(tu => tu.TriggeredAssignmentRuleHistories).HasForeignKey(ah => ah.TriggeredByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}