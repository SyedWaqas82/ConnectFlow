using ConnectFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class SubscriptionConfiguration : BaseAuditableConfiguration<Subscription>
{
    public override void Configure(EntityTypeBuilder<Subscription> builder)
    {
        base.Configure(builder);

        //builder.Property(ts => ts.MonthlyPrice).HasPrecision(18, 2);

        // Configure relationships
        builder.HasOne(ts => ts.Tenant).WithMany(t => t.Subscriptions).HasForeignKey(ts => ts.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}