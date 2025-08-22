using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class PlanConfiguration : BaseAuditableConfiguration<Plan>
{
    public override void Configure(EntityTypeBuilder<Plan> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.PaymentProviderPriceId).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.Type).IsRequired();
        builder.Property(p => p.BillingCycle).IsRequired();

        // Configure relationships
        builder.HasMany(p => p.Subscriptions).WithOne(s => s.Plan).HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Restrict);

        SeedPlans(builder);
    }

    private static void SeedPlans(EntityTypeBuilder<Plan> builder)
    {
        var plans = new[]
        {
            new Plan
            {
                Id = 1,
                Name = "Free",
                PaymentProviderPriceId = "price_free", // Replace with actual payment provider price ID
                Price = 0,
                Type = PlanType.Free,
                BillingCycle = BillingCycle.Monthly,
                MaxUsers = 2,
                MaxChannels = 1,
                MaxWhatsAppChannels = 1,
                MaxFacebookChannels = 0,
                MaxInstagramChannels = 0,
                MaxTelegramChannels = 0,
                IsActive = true
            },
            new Plan
            {
                Id = 2,
                Name = "Starter",
                PaymentProviderPriceId = "price_starter_monthly", // Replace with actual payment provider price ID
                Price = 29.99m,
                Type = PlanType.Starter,
                BillingCycle = BillingCycle.Monthly,
                MaxUsers = 5,
                MaxChannels = 3,
                MaxWhatsAppChannels = 2,
                MaxFacebookChannels = 1,
                MaxInstagramChannels = 1,
                MaxTelegramChannels = 1,
                IsActive = true
            },
            new Plan
            {
                Id = 3,
                Name = "Professional",
                PaymentProviderPriceId = "price_pro_monthly", // Replace with actual payment provider price ID
                Price = 99.99m,
                Type = PlanType.Pro,
                BillingCycle = BillingCycle.Monthly,
                MaxUsers = 25,
                MaxChannels = 10,
                MaxWhatsAppChannels = 5,
                MaxFacebookChannels = 3,
                MaxInstagramChannels = 3,
                MaxTelegramChannels = 3,
                IsActive = true
            },
            new Plan
            {
                Id = 4,
                Name = "Enterprise",
                PaymentProviderPriceId = "price_enterprise_monthly", // Replace with actual payment provider price ID
                Price = 299.99m,
                Type = PlanType.Enterprise,
                BillingCycle = BillingCycle.Monthly,
                MaxUsers = 100,
                MaxChannels = 50,
                MaxWhatsAppChannels = 20,
                MaxFacebookChannels = 15,
                MaxInstagramChannels = 15,
                MaxTelegramChannels = 15,
                IsActive = true
            }
        };

        builder.HasData(plans);
    }
}