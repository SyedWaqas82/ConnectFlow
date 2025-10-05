using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class SequenceStepConfiguration : BaseAuditableConfiguration<SequenceStep>
{
    public override void Configure(EntityTypeBuilder<SequenceStep> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(ss => ss.Name).IsRequired().HasMaxLength(200);
        builder.Property(ss => ss.Description).HasMaxLength(2000);
        builder.Property(ss => ss.StepType).IsRequired().HasConversion<string>();
        builder.Property(ss => ss.StepOrder).IsRequired();
        builder.Property(ss => ss.DelayDays).IsRequired();
        builder.Property(ss => ss.DelayMinutes).IsRequired();
        builder.Property(ss => ss.Subject).HasMaxLength(500);
        builder.Property(ss => ss.Note).HasMaxLength(4000);
        builder.Property(ss => ss.Type).IsRequired().HasConversion<string>();
        builder.Property(ss => ss.Priority).IsRequired().HasConversion<string>();
        builder.Property(ss => ss.ActivityOwnerId).IsRequired();

        // Configure relationships
        builder.HasOne(ss => ss.Sequence).WithMany(s => s.Steps).HasForeignKey(ss => ss.SequenceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(ss => ss.CurrentEnrollments).WithOne(e => e.CurrentStep).HasForeignKey(e => e.CurrentStepId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(ss => ss.Activities).WithOne(a => a.SequenceStep).HasForeignKey(a => a.SequenceStepId).OnDelete(DeleteBehavior.SetNull);
    }
}