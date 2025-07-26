using ConnectFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class SubscriptionConfiguration : BaseAuditableConfiguration<Subscription>
{
    public override void Configure(EntityTypeBuilder<Subscription> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.Currency).HasMaxLength(3).IsRequired();
        builder.Property(s => s.Plan).IsRequired();
        builder.Property(s => s.Status).IsRequired();
        builder.Property(s => s.TenantId).IsRequired();
        builder.HasIndex(s => s.TenantId);

        // Configure relationships
        builder.HasOne(s => s.Tenant).WithMany(t => t.Subscriptions).HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}