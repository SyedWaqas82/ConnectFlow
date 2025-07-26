using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Entities;
using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Infrastructure.Services;

public class StripeService : IStripeService
{
    //     private readonly IConfiguration _configuration;
    //     private readonly IApplicationDbContext _context;

    //     public StripeService(
    //         IConfiguration configuration,
    //         IApplicationDbContext context)
    //     {
    //         _configuration = configuration;
    //         _context = context;
    //         StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    //     }

    //     public async Task<Subscription> CreateSubscriptionAsync(
    //         string tenantId,
    //         string email,
    //         string paymentMethodId,
    //         SubscriptionPlan plan)
    //     {
    //         // Create or get Stripe customer
    //         var customerService = new CustomerService();
    //         var customer = await customerService.CreateAsync(new CustomerCreateOptions
    //         {
    //             Email = email,
    //             PaymentMethod = paymentMethodId,
    //             InvoiceSettings = new CustomerInvoiceSettingsOptions
    //             {
    //                 DefaultPaymentMethod = paymentMethodId
    //             }
    //         });

    //         // Create Stripe subscription
    //         var subscriptionService = new SubscriptionService();
    //         var subscription = await subscriptionService.CreateAsync(new SubscriptionCreateOptions
    //         {
    //             Customer = customer.Id,
    //             Items = new List<SubscriptionItemOptions>
    //             {
    //                 new() { Price = GetPriceIdForPlan(plan) }
    //             },
    //             PaymentSettings = new SubscriptionPaymentSettingsOptions
    //             {
    //                 PaymentMethodTypes = new List<string> { "card" }
    //             },
    //             TrialPeriodDays = GetTrialDaysForPlan(plan)
    //         });

    //         // Create local subscription record
    //         var localSubscription = new Subscription
    //         {
    //             TenantId = tenantId,
    //             StripeCustomerId = customer.Id,
    //             StripeSubscriptionId = subscription.Id,
    //             Plan = plan,
    //             Status = subscription.Status == "trialing" ? SubscriptionStatus.Trialing : SubscriptionStatus.Active,
    //             TrialEndsAt = subscription.TrialEnd?.ToUniversalTime(),
    //             CurrentPeriodStartsAt = subscription.CurrentPeriodStart.ToUniversalTime(),
    //             CurrentPeriodEndsAt = subscription.CurrentPeriodEnd.ToUniversalTime(),
    //             ContactLimit = GetContactLimitForPlan(plan),
    //             UserLimit = GetUserLimitForPlan(plan),
    //             StorageLimit = GetStorageLimitForPlan(plan),
    //             MonthlyEmailLimit = GetEmailLimitForPlan(plan),
    //             MonthlySMSLimit = GetSMSLimitForPlan(plan),
    //             MonthlyWhatsAppLimit = GetWhatsAppLimitForPlan(plan)
    //         };

    //         _context.Subscriptions.Add(localSubscription);
    //         await _context.SaveChangesAsync(CancellationToken.None);

    //         return localSubscription;
    //     }

    //     public async Task CancelSubscriptionAsync(string subscriptionId, string? reason = null)
    //     {
    //         var subscriptionService = new SubscriptionService();
    //         await subscriptionService.CancelAsync(subscriptionId, new SubscriptionCancelOptions());

    //         var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
    //         if (subscription != null)
    //         {
    //             subscription.Status = SubscriptionStatus.Canceled;
    //             subscription.CanceledAt = DateTimeOffset.UtcNow;
    //             subscription.CancellationReason = reason;
    //             await _context.SaveChangesAsync(CancellationToken.None);
    //         }
    //     }

    //     private string GetPriceIdForPlan(SubscriptionPlan plan) => plan switch
    //     {
    //         SubscriptionPlan.Starter => _configuration["Stripe:Prices:Starter"]!,
    //         SubscriptionPlan.Professional => _configuration["Stripe:Prices:Professional"]!,
    //         SubscriptionPlan.Business => _configuration["Stripe:Prices:Business"]!,
    //         SubscriptionPlan.Enterprise => _configuration["Stripe:Prices:Enterprise"]!,
    //         _ => throw new ArgumentException("Invalid plan", nameof(plan))
    //     };

    //     private int GetTrialDaysForPlan(SubscriptionPlan plan) => plan switch
    //     {
    //         SubscriptionPlan.Starter => 14,
    //         SubscriptionPlan.Professional => 14,
    //         SubscriptionPlan.Business => 30,
    //         SubscriptionPlan.Enterprise => 30,
    //         _ => 0
    //     };

    //     private int GetContactLimitForPlan(SubscriptionPlan plan) => plan switch
    //     {
    //         SubscriptionPlan.Free => 100,
    //         SubscriptionPlan.Starter => 1000,
    //         SubscriptionPlan.Professional => 10000,
    //         SubscriptionPlan.Business => 50000,
    //         SubscriptionPlan.Enterprise => 1000000,
    //         _ => 0
    //     };

    //     private int GetUserLimitForPlan(SubscriptionPlan plan) => plan switch
    //     {
    //         SubscriptionPlan.Free => 2,
    //         SubscriptionPlan.Starter => 5,
    //         SubscriptionPlan.Professional => 15,
    //         SubscriptionPlan.Business => 50,
    //         SubscriptionPlan.Enterprise => 200,
    //         _ => 0
    //     };

    //     private int GetStorageLimitForPlan(SubscriptionPlan plan) => plan switch
    //     {
    //         SubscriptionPlan.Free => 100,      // 100MB
    //         SubscriptionPlan.Starter => 1024,   // 1GB
    //         SubscriptionPlan.Professional => 5120,  // 5GB
    //         SubscriptionPlan.Business => 20480,    // 20GB
    //         SubscriptionPlan.Enterprise => 102400,  // 100GB
    //         _ => 0
    //     };

    //     private int GetEmailLimitForPlan(SubscriptionPlan plan) => plan switch
    //     {
    //         SubscriptionPlan.Free => 100,
    //         SubscriptionPlan.Starter => 1000,
    //         SubscriptionPlan.Professional => 10000,
    //         SubscriptionPlan.Business => 50000,
    //         SubscriptionPlan.Enterprise => 200000,
    //         _ => 0
    //     };

    //     private int GetSMSLimitForPlan(SubscriptionPlan plan) => plan switch
    //     {
    //         SubscriptionPlan.Free => 0,
    //         SubscriptionPlan.Starter => 100,
    //         SubscriptionPlan.Professional => 1000,
    //         SubscriptionPlan.Business => 5000,
    //         SubscriptionPlan.Enterprise => 20000,
    //         _ => 0
    //     };

    //     private int GetWhatsAppLimitForPlan(SubscriptionPlan plan) => plan switch
    //     {
    //         SubscriptionPlan.Free => 0,
    //         SubscriptionPlan.Starter => 100,
    //         SubscriptionPlan.Professional => 1000,
    //         SubscriptionPlan.Business => 5000,
    //         SubscriptionPlan.Enterprise => 20000,
    //         _ => 0
    //     };
    public Task CancelSubscriptionAsync(string subscriptionId, string? reason = null)
    {
        throw new NotImplementedException();
    }

    public Task<Subscription> CreateSubscriptionAsync(string tenantId, string email, string paymentMethodId, SubscriptionPlan plan)
    {
        throw new NotImplementedException();
    }
}
