using ConnectFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class TenantUserRoleConfiguration : BaseAuditableConfiguration<TenantUserRole>
{
    public override void Configure(EntityTypeBuilder<TenantUserRole> builder)
    {
        base.Configure(builder);

        builder.Property(tur => tur.RoleName).IsRequired().HasMaxLength(100);
        builder.Property(tur => tur.AssignedAt).IsRequired();

        builder.HasIndex(tur => new { tur.TenantUserId, tur.RoleName }).IsUnique();

        // Configure relationships
        builder.HasOne(tur => tur.TenantUser).WithMany(tu => tu.TenantUserRoles).HasForeignKey(tur => tur.TenantUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(tur => tur.AssignedBy).OnDelete(DeleteBehavior.SetNull);
    }
}