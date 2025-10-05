using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class SchedulerBookingConfiguration : BaseAuditableConfiguration<SchedulerBooking>
{
    public override void Configure(EntityTypeBuilder<SchedulerBooking> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(sb => sb.BookerEmail).HasMaxLength(256);
        builder.Property(sb => sb.BookerName).HasMaxLength(256);
        builder.Property(sb => sb.BookerPhone).HasMaxLength(20);
        builder.Property(sb => sb.BookingFormData).HasColumnType("jsonb");
        builder.Property(sb => sb.BookingSource).HasMaxLength(100);
        builder.Property(sb => sb.CancellationReason).HasMaxLength(1000);

        // Configure relationships
        builder.HasOne(sb => sb.SchedulerSlot).WithMany(ss => ss.Bookings).HasForeignKey(sb => sb.SchedulerSlotId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(sb => sb.CancelledBy).WithMany().HasForeignKey(sb => sb.CancelledById).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(sb => sb.RescheduledFromBooking).WithOne().HasForeignKey<SchedulerBooking>(sb => sb.RescheduledFromBookingId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(sb => sb.RescheduledToBooking).WithOne().HasForeignKey<SchedulerBooking>(sb => sb.RescheduledToBookingId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(sb => sb.Activities).WithOne(a => a.SchedulerBooking).HasForeignKey(a => a.SchedulerBookingId).OnDelete(DeleteBehavior.SetNull);
    }
}