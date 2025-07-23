using ConnectFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class TenantUserConfiguration : BaseAuditableConfiguration<TenantUser>
{
    public override void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        base.Configure(builder);

        builder.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
        builder.Property(tu => tu.UserId).IsRequired();
        builder.Property(tu => tu.IsActive).IsRequired();
        builder.Property(tu => tu.JoinedAt).IsRequired();
        builder.Property(tu => tu.Status).IsRequired();
        builder.Property(tu => tu.TenantId).IsRequired();
        builder.Property(tu => tu.InvitedBy).IsRequired(false);
        builder.Property(tu => tu.LeftAt).IsRequired(false);

        // Configure relationships
        builder.HasMany(tu => tu.TenantUserRoles).WithOne(tur => tur.TenantUser).HasForeignKey(tur => tur.TenantUserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(tu => tu.Tenant).WithMany(t => t.TenantUsers).HasForeignKey(tu => tu.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}