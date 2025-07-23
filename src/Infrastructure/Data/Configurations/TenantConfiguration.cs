using ConnectFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class TenantConfiguration : BaseAuditableConfiguration<Tenant>
{
    public override void Configure(EntityTypeBuilder<Tenant> builder)
    {
        base.Configure(builder);

        builder.HasIndex(e => e.Code).IsUnique();
        builder.HasIndex(e => e.Domain).IsUnique();
        builder.Property(e => e.Code).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Domain).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Avatar).HasMaxLength(200);
        builder.Property(e => e.Phone).HasMaxLength(20);
        builder.Property(e => e.Email).HasMaxLength(100);
        builder.Property(e => e.Website).HasMaxLength(200);
        builder.Property(e => e.Address).HasMaxLength(200);
        builder.Property(e => e.City).HasMaxLength(100);
        builder.Property(e => e.State).HasMaxLength(100);
        builder.Property(e => e.Country).HasMaxLength(100);
        builder.Property(e => e.PostalCode).HasMaxLength(20);
        builder.Property(e => e.Settings).HasColumnType("jsonb"); // Assuming PostgreSQL, adjust for other DBs
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.DeactivatedAt).HasDefaultValue(null);

        // Configure relationships
        builder.HasMany(t => t.TenantUsers).WithOne(tu => tu.Tenant).HasForeignKey(tu => tu.TenantId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(t => t.Subscriptions).WithOne(s => s.Tenant).HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}