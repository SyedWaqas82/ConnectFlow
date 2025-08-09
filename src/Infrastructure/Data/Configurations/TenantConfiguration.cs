using ConnectFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class TenantConfiguration : BaseAuditableConfiguration<Tenant>
{
    public override void Configure(EntityTypeBuilder<Tenant> builder)
    {
        base.Configure(builder);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Domain).HasMaxLength(100);
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.Avatar).HasMaxLength(200);
        builder.Property(t => t.Phone).HasMaxLength(20);
        builder.Property(t => t.Email).HasMaxLength(100);
        builder.Property(t => t.Website).HasMaxLength(200);
        builder.Property(t => t.Address).HasMaxLength(200);
        builder.Property(t => t.City).HasMaxLength(100);
        builder.Property(t => t.State).HasMaxLength(100);
        builder.Property(t => t.Country).HasMaxLength(100);
        builder.Property(t => t.PostalCode).HasMaxLength(20);
        builder.Property(t => t.Settings).HasColumnType("jsonb"); // Assuming PostgreSQL, adjust for other DBs
        builder.Property(t => t.DeactivatedAt).HasDefaultValue(null);

        // Configure relationships
        builder.HasMany(t => t.TenantUsers).WithOne(tu => tu.Tenant).HasForeignKey(tu => tu.TenantId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(t => t.Subscriptions).WithOne(s => s.Tenant).HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}