using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ChannelAccountConfiguration : BaseAuditableConfiguration<ChannelAccount>
{
    public override void Configure(EntityTypeBuilder<ChannelAccount> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(ca => ca.Type).IsRequired().HasConversion<int>();
        builder.Property(ca => ca.Status).IsRequired().HasConversion<int>();
        builder.Property(ca => ca.ProviderAccountId).IsRequired(false).HasMaxLength(100);
        builder.Property(ca => ca.DisplayName).IsRequired(false).HasMaxLength(200);
        builder.Property(ca => ca.Contact).IsRequired(false).HasMaxLength(100);
        builder.Property(ca => ca.SettingsJson).IsRequired(false).HasColumnType("jsonb");

        builder.HasIndex(ca => new { ca.TenantId, ca.ProviderAccountId }).IsUnique().HasDatabaseName("IX_ChannelAccount_TenantId_ProviderAccountId");

        // Configure relationships
    }
}