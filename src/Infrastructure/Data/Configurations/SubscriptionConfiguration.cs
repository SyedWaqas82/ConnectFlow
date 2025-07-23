using ConnectFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class SubscriptionConfiguration : BaseAuditableConfiguration<Subscription>
{
    public override void Configure(EntityTypeBuilder<Subscription> builder)
    {
        base.Configure(builder);

        builder.Property(ts => ts.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(ts => ts.Currency).HasMaxLength(3).IsRequired();
        builder.Property(ts => ts.Plan).IsRequired();
        builder.Property(ts => ts.Status).IsRequired();

        // Configure relationships
        builder.HasOne(ts => ts.Tenant).WithMany(t => t.Subscriptions).HasForeignKey(ts => ts.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}