using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class TenantConfiguration : BaseAuditableConfiguration<Tenant>
{
    public override void Configure(EntityTypeBuilder<Tenant> builder)
    {
        base.Configure(builder);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Domain).HasMaxLength(100);
        builder.Property(t => t.PaymentProviderCustomerId).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Email).HasMaxLength(256).IsRequired();
        builder.Property(t => t.Settings).HasColumnType("jsonb"); // Assuming PostgreSQL, adjust for other DBs
        builder.Property(t => t.DeactivatedAt).HasDefaultValue(null);

        builder.HasIndex(t => t.PaymentProviderCustomerId).IsUnique();

        // Configure relationships
        builder.HasMany(t => t.TenantUsers).WithOne(tu => tu.Tenant).HasForeignKey(tu => tu.TenantId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(t => t.Subscriptions).WithOne(s => s.Tenant).HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}