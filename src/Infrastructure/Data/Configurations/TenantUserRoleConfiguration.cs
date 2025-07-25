using System;
using ConnectFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class TenantUserRoleConfiguration : BaseAuditableConfiguration<TenantUserRole>
{
    public override void Configure(EntityTypeBuilder<TenantUserRole> builder)
    {
        base.Configure(builder);

        builder.HasIndex(e => new { e.TenantUserId, e.RoleName }).IsUnique();
        builder.Property(tur => tur.RoleName).IsRequired().HasMaxLength(100);
        builder.Property(tur => tur.IsActive).IsRequired();
        builder.Property(tur => tur.AssignedAt).IsRequired();
        builder.Property(tur => tur.AssignedBy).IsRequired(false);
        builder.Property(tur => tur.RevokedAt).IsRequired(false);

        // Configure relationships
        builder.HasOne(tur => tur.TenantUser).WithMany(tu => tu.TenantUserRoles).HasForeignKey(tur => tur.TenantUserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(tur => tur.AssignedByUser).WithMany().HasForeignKey(tur => tur.AssignedBy).OnDelete(DeleteBehavior.SetNull);
    }
}