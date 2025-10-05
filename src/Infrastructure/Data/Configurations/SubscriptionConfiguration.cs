using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class SubscriptionConfiguration : BaseAuditableConfiguration<Subscription>
{
    public override void Configure(EntityTypeBuilder<Subscription> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.PaymentProviderSubscriptionId).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Status).IsRequired().HasConversion<string>();
        builder.Property(s => s.CurrentPeriodStart).IsRequired();
        builder.Property(s => s.CurrentPeriodEnd).IsRequired();
        builder.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(s => s.Currency).IsRequired().HasMaxLength(3);

        builder.Property(s => s.TenantId).IsRequired();
        builder.HasIndex(s => s.TenantId);

        builder.HasIndex(s => s.PaymentProviderSubscriptionId).IsUnique();
        builder.HasIndex(s => new { s.TenantId, s.Status });

        // Configure relationships
        builder.HasOne(s => s.Plan).WithMany(p => p.Subscriptions).HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Tenant).WithMany(t => t.Subscriptions).HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}