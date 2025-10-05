using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class SequenceConfiguration : BaseAuditableConfiguration<Sequence>
{
    public override void Configure(EntityTypeBuilder<Sequence> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Description).HasMaxLength(2000);
        builder.Property(s => s.TargetType).IsRequired().HasConversion<string>();
        builder.Property(s => s.Settings).HasColumnType("jsonb");

        // Configure relationships
        builder.HasOne(s => s.Owner).WithMany(tu => tu.Sequences).HasForeignKey(s => s.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(s => s.Steps).WithOne(ss => ss.Sequence).HasForeignKey(ss => ss.SequenceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.Enrollments).WithOne(e => e.Sequence).HasForeignKey(e => e.SequenceId).OnDelete(DeleteBehavior.Cascade);
    }
}