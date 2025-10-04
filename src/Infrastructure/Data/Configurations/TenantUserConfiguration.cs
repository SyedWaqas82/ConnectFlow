using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class TenantUserConfiguration : BaseAuditableConfiguration<TenantUser>
{
    public override void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        base.Configure(builder);

        builder.Property(tu => tu.ApplicationUserId).IsRequired();
        builder.Property(tu => tu.Status).IsRequired().HasConversion<string>();
        builder.Property(tu => tu.JoinedAt).IsRequired();
        builder.Property(tu => tu.InvitedBy).IsRequired(false);
        builder.Property(tu => tu.LeftAt).IsRequired(false);
        builder.Property(au => au.TimeZone).IsRequired().HasMaxLength(50);
        builder.Property(au => au.Language).IsRequired().HasMaxLength(10);
        builder.Property(au => au.DateNumberFormat).IsRequired().HasMaxLength(20);
        builder.Property(au => au.DefaultCurrency).IsRequired().HasMaxLength(10);
        builder.Property(au => au.Settings).HasColumnType("jsonb"); // Store preferences as JSON
        builder.Property(tu => tu.TenantId).IsRequired();
        builder.HasIndex(tu => tu.TenantId);
        builder.HasIndex(tu => new { tu.TenantId, tu.ApplicationUserId }).IsUnique();

        // Configure relationships
        builder.HasMany(tu => tu.TenantUserRoles).WithOne(tur => tur.TenantUser).HasForeignKey(tur => tur.TenantUserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(tu => tu.Tenant).WithMany(t => t.TenantUsers).HasForeignKey(tu => tu.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}