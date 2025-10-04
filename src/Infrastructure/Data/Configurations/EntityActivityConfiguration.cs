using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityActivityConfiguration : BaseAuditableConfiguration<EntityActivity>
{
    public override void Configure(EntityTypeBuilder<EntityActivity> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(a => a.Subject).IsRequired().HasMaxLength(255);
        builder.Property(a => a.Type).IsRequired().HasConversion<string>();
        builder.Property(a => a.Note).HasMaxLength(4000);
        builder.Property(a => a.Priority).IsRequired().HasConversion<string>();
        builder.Property(a => a.Location).HasMaxLength(500);
        builder.Property(a => a.ConferenceUrl).HasMaxLength(1000);
        builder.Property(a => a.Description).HasMaxLength(4000);
        builder.Property(a => a.VisibilityOnCalendar).IsRequired().HasConversion<string>();
        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.EntityType).IsRequired().HasConversion<string>();

        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId }).HasDatabaseName("IX_Activity_TenantId_EntityType_EntityId");

        // Configure relationships
        builder.HasOne(a => a.AssignedBy).WithMany(tu => tu.AssignedByActivities).HasForeignKey(a => a.AssignedById).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.AssignedTo).WithMany(tu => tu.AssignedActivities).HasForeignKey(a => a.AssignedToId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(a => a.Participants).WithOne(p => p.Activity).HasForeignKey(p => p.ActivityId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.SequenceStep).WithMany(ss => ss.Activities).HasForeignKey(a => a.SequenceStepId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(a => a.SchedulerBooking).WithMany(sb => sb.Activities).HasForeignKey(a => a.SchedulerBookingId).OnDelete(DeleteBehavior.SetNull);
    }
}