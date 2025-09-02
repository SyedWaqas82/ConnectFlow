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
                Description = "Basic plan with limited features",
                Currency = "usd",
                PaymentProviderPriceId = "price_free",
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
                Name = "Starter Plan - Monthly",
                Description = "Starter plan with basic features",
                Currency = "usd",
                PaymentProviderProductId = "prod_SxgcF8F4u3unNk",
                PaymentProviderPriceId = "price_1S1lFgDVRyfs46JiBJyvA5eu",
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
                Name = "Starter Plan - Yearly",
                Description = "Starter plan with basic features",
                Currency = "usd",
                PaymentProviderProductId = "prod_Sxgf0MdFfTzUXR",
                PaymentProviderPriceId = "price_1S1lHtDVRyfs46JizuWqnOp2",
                Price = 299.99m,
                Type = PlanType.Starter,
                BillingCycle = BillingCycle.Yearly,
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
                Id = 4,
                Name = "Professional Plan - Monthly",
                Description = "Professional plan with advanced features",
                Currency = "usd",
                PaymentProviderProductId = "prod_SxgfAcz4HHgFcY",
                PaymentProviderPriceId = "price_1S1lIXDVRyfs46Jirxqm0dz6",
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
                Id = 5,
                Name = "Professional Plan - Yearly",
                Description = "Professional plan with advanced features",
                Currency = "usd",
                PaymentProviderProductId = "prod_SxggEc36SZchwA",
                PaymentProviderPriceId = "price_1S1lJ3DVRyfs46Ji40RP91Sk",
                Price = 999.99m,
                Type = PlanType.Pro,
                BillingCycle = BillingCycle.Yearly,
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
                Id = 6,
                Name = "Enterprise Plan - Monthly",
                Description = "Enterprise plan with all features",
                Currency = "usd",
                PaymentProviderProductId = "prod_Sxgh4Ucpw7IxSG",
                PaymentProviderPriceId = "price_1S1lJgDVRyfs46JidlIn73va",
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
            },
            new Plan
            {
                Id = 7,
                Name = "Enterprise Plan - Yearly",
                Description = "Enterprise plan with all features",
                Currency = "usd",
                PaymentProviderProductId = "prod_SxghGjm7I9Ugag",
                PaymentProviderPriceId = "price_1S1lKVDVRyfs46Ji1DJXRhHp",
                Price = 2999.99m,
                Type = PlanType.Enterprise,
                BillingCycle = BillingCycle.Yearly,
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