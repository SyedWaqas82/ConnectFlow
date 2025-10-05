using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class SchedulerConfiguration : BaseAuditableConfiguration<Scheduler>
{
    public override void Configure(EntityTypeBuilder<Scheduler> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Description).HasMaxLength(2000);
        builder.Property(s => s.AvailableFrom).IsRequired();
        builder.Property(s => s.BeforeEventBufferTimeMinutes).IsRequired();
        builder.Property(s => s.AfterEventBufferTimeMinutes).IsRequired();
        builder.Property(s => s.StartingIntervalMinutes).IsRequired().HasConversion<string>();
        builder.Property(s => s.MinimumNoticeToBook).IsRequired().HasConversion<string>();
        builder.Property(s => s.FurthestNoticeToBookInFutureDays).IsRequired().HasConversion<string>();
        builder.Property(s => s.Timezone).IsRequired().HasMaxLength(50);
        builder.Property(s => s.DefaultSubject).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Type).IsRequired().HasConversion<string>();
        builder.Property(s => s.MeetingNote).HasMaxLength(4000);
        builder.Property(s => s.DefaultLocation).HasMaxLength(500);
        builder.Property(s => s.DefaultConferenceUrl).HasMaxLength(1000);
        builder.Property(s => s.MeetingDescription).HasMaxLength(4000);
        builder.Property(s => s.VisibleCompanyInfo).HasMaxLength(2000);
        builder.Property(s => s.CustomBookingFormFields).HasColumnType("jsonb");
        builder.Property(s => s.FooterNote).HasMaxLength(2000);
        builder.Property(s => s.UrlSlug).HasMaxLength(100).IsUnicode(false);

        // Configure relationships
        builder.HasOne(s => s.Owner).WithMany(tu => tu.Schedulers).HasForeignKey(s => s.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(s => s.Availabilities).WithOne(sa => sa.Scheduler).HasForeignKey(sa => sa.SchedulerId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.ManualSlots).WithOne(ss => ss.Scheduler).HasForeignKey(ss => ss.SchedulerId).OnDelete(DeleteBehavior.Cascade);
    }
}