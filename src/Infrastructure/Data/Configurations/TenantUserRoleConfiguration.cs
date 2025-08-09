using ConnectFlow.Domain.Entities;
using ConnectFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class TenantUserRoleConfiguration : BaseAuditableConfiguration<TenantUserRole>
{
    public override void Configure(EntityTypeBuilder<TenantUserRole> builder)
    {
        base.Configure(builder);

        builder.Property(tur => tur.RoleName).IsRequired().HasMaxLength(100);
        builder.Property(tur => tur.AssignedAt).IsRequired();
        builder.Property(tur => tur.AssignedBy).IsRequired(false);
        builder.Property(tur => tur.RevokedAt).IsRequired(false);
        builder.HasIndex(tur => new { tur.TenantUserId, tur.RoleName }).IsUnique();

        // Configure relationships
        builder.HasOne(tur => tur.TenantUser).WithMany(tu => tu.TenantUserRoles).HasForeignKey(tur => tur.TenantUserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(tur => tur.AssignedBy).OnDelete(DeleteBehavior.SetNull);
    }
}