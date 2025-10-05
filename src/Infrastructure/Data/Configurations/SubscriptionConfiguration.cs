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
        builder.HasIndex(s => s.TenantId).HasDatabaseName("IX_Subscription_TenantId");

        builder.HasIndex(s => s.PaymentProviderSubscriptionId).IsUnique().HasDatabaseName("IX_Subscription_PaymentProviderSubscriptionId");
        builder.HasIndex(s => new { s.TenantId, s.Status }).HasDatabaseName("IX_Subscription_TenantId_Status");

        // Add indexes for payment and period date filtering
        builder.HasIndex(s => s.PlanId).HasDatabaseName("IX_Subscription_PlanId");
        builder.HasIndex(s => new { s.TenantId, s.CurrentPeriodEnd }).HasDatabaseName("IX_Subscription_TenantId_CurrentPeriodEnd");
        builder.HasIndex(s => new { s.Status, s.CurrentPeriodEnd }).HasDatabaseName("IX_Subscription_Status_CurrentPeriodEnd");
        builder.HasIndex(s => s.CanceledAt).HasDatabaseName("IX_Subscription_CanceledAt");
        builder.HasIndex(s => s.IsInGracePeriod).HasDatabaseName("IX_Subscription_IsInGracePeriod");

        // Configure relationships
        builder.HasOne(s => s.Plan).WithMany(p => p.Subscriptions).HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Tenant).WithMany(t => t.Subscriptions).HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Restrict);
    }
}