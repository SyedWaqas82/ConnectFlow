using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class PlanConfiguration : BaseAuditableConfiguration<Plan>
{
    public override void Configure(EntityTypeBuilder<Plan> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.StripePriceId).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.Type).IsRequired();
        builder.Property(p => p.BillingCycle).IsRequired();

        // Configure relationships
        builder.HasMany(p => p.Subscriptions).WithOne(s => s.Plan).HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Restrict);
    }
}