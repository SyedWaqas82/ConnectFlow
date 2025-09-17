using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ChangeLog entity
/// </summary>
public class ChangeLogConfiguration : BaseAuditableConfiguration<ChangeLog>
{
    public override void Configure(EntityTypeBuilder<ChangeLog> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(e => e.ChangeType).IsRequired();
        builder.Property(e => e.PropertyName).HasMaxLength(100);
        builder.Property(e => e.PropertyDisplayName).HasMaxLength(200);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.OldValue).HasMaxLength(4000);
        builder.Property(e => e.NewValue).HasMaxLength(4000);
        builder.Property(e => e.Metadata).HasColumnType("nvarchar(max)");
        builder.Property(e => e.Context).HasMaxLength(500);
        builder.Property(e => e.IpAddress).HasMaxLength(45); // Supports IPv6
        builder.Property(e => e.UserAgent).HasMaxLength(500);

        // Configure relationships
    }
}