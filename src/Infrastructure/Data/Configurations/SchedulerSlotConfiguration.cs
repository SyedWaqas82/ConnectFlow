using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class SchedulerSlotConfiguration : BaseAuditableConfiguration<SchedulerSlot>
{
    public override void Configure(EntityTypeBuilder<SchedulerSlot> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(ss => ss.Title).IsRequired().HasMaxLength(200);
        builder.Property(ss => ss.Description).HasMaxLength(1000);
        builder.Property(ss => ss.SlotType).IsRequired().HasConversion<string>();
        builder.Property(ss => ss.StartDateTime).IsRequired();
        builder.Property(ss => ss.EndDateTime).IsRequired();

        // Configure relationships
        builder.HasOne(ss => ss.Scheduler).WithMany(s => s.ManualSlots).HasForeignKey(ss => ss.SchedulerId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ss => ss.Availability).WithMany(sa => sa.GeneratedSlots).HasForeignKey(ss => ss.AvailabilityId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(ss => ss.Bookings).WithOne(sb => sb.SchedulerSlot).HasForeignKey(sb => sb.SchedulerSlotId).OnDelete(DeleteBehavior.Restrict);
    }
}