using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class SchedulerAvailabilityConfiguration : BaseAuditableConfiguration<SchedulerAvailability>
{
    public override void Configure(EntityTypeBuilder<SchedulerAvailability> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(sa => sa.DayOfWeek).IsRequired().HasConversion<string>();
        builder.Property(sa => sa.StartTime).IsRequired();
        builder.Property(sa => sa.EndTime).IsRequired();
        builder.Property(sa => sa.SchedulerId).IsRequired();

        // Configure relationships
        builder.HasOne(sa => sa.Scheduler).WithMany(s => s.Availabilities).HasForeignKey(sa => sa.SchedulerId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(sa => sa.GeneratedSlots).WithOne(ss => ss.Availability).HasForeignKey(ss => ss.AvailabilityId).OnDelete(DeleteBehavior.Cascade);
    }
}