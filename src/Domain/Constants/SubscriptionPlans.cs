namespace ConnectFlow.Domain.Constants;

using ConnectFlow.Domain.Entities;

public static class SubscriptionPlans
{
    // Private templates - not for direct use outside this class
    private static readonly Subscription _freePlan = new Subscription
    {
        UserLimit = 1,
        WhatsAppAccountLimit = 1,
        TotalAccountLimit = 1,
        Amount = 0,
        Currency = "USD",
        Plan = SubscriptionPlan.Free,
        BillingCycle = BillingCycle.Monthly,
        Status = SubscriptionStatus.Active,
        CurrentPeriodStartsAt = DateTimeOffset.UtcNow,
        CurrentPeriodEndsAt = DateTimeOffset.UtcNow.AddMonths(1),
    };

    private static readonly Subscription _starterPlan = new Subscription
    {
        UserLimit = 3,
        WhatsAppAccountLimit = 3,
        TotalAccountLimit = 3,
        Amount = 29,
        Currency = "USD",
        Plan = SubscriptionPlan.Starter,
        BillingCycle = BillingCycle.Monthly,
        Status = SubscriptionStatus.Active,
        CurrentPeriodStartsAt = DateTimeOffset.UtcNow,
        CurrentPeriodEndsAt = DateTimeOffset.UtcNow.AddMonths(1),
    };

    private static readonly Subscription _professionalPlan = new Subscription
    {
        UserLimit = 5,
        WhatsAppAccountLimit = 5,
        TotalAccountLimit = 5,
        Amount = 99,
        Currency = "USD",
        Plan = SubscriptionPlan.Professional,
        BillingCycle = BillingCycle.Monthly,
        Status = SubscriptionStatus.Active,
        CurrentPeriodStartsAt = DateTimeOffset.UtcNow,
        CurrentPeriodEndsAt = DateTimeOffset.UtcNow.AddMonths(1),
    };

    private static readonly Subscription _enterprisePlan = new Subscription
    {
        UserLimit = int.MaxValue,
        WhatsAppAccountLimit = int.MaxValue,
        TotalAccountLimit = int.MaxValue,
        Amount = 499,
        Currency = "USD",
        Plan = SubscriptionPlan.Enterprise,
        BillingCycle = BillingCycle.Monthly,
        Status = SubscriptionStatus.Active,
        CurrentPeriodStartsAt = DateTimeOffset.UtcNow,
        CurrentPeriodEndsAt = DateTimeOffset.UtcNow.AddMonths(1),
    };

    private static readonly Subscription _enterpriseUnlimitedPlan = new Subscription
    {
        UserLimit = int.MaxValue,
        WhatsAppAccountLimit = int.MaxValue,
        TotalAccountLimit = int.MaxValue,
        Amount = 0,
        Currency = "USD",
        Plan = SubscriptionPlan.Enterprise,
        BillingCycle = BillingCycle.Monthly,
        Status = SubscriptionStatus.Active,
        CurrentPeriodStartsAt = DateTimeOffset.UtcNow,
    };

    // Public properties that return a new instance each time they are accessed
    public static Subscription Free => GetPlanByName(SubscriptionPlan.Free);
    public static Subscription Starter => GetPlanByName(SubscriptionPlan.Starter);
    public static Subscription Professional => GetPlanByName(SubscriptionPlan.Professional);
    public static Subscription Enterprise => GetPlanByName(SubscriptionPlan.Enterprise);
    public static Subscription EnterpriseUnlimited => GetPlanByName(SubscriptionPlan.Enterprise);

    public static Subscription GetPlanByName(SubscriptionPlan plan)
    {
        // Get the template subscription based on plan
        Subscription template = plan switch
        {
            SubscriptionPlan.Free => _freePlan,
            SubscriptionPlan.Starter => _starterPlan,
            SubscriptionPlan.Professional => _professionalPlan,
            SubscriptionPlan.Enterprise => _enterprisePlan,
            _ => throw new ArgumentException($"Unknown subscription plan: {plan}")
        };

        // Return a new instance with the template's properties, but without ID
        return new Subscription
        {
            // Do NOT copy Id - let the database generate it
            UserLimit = template.UserLimit,
            WhatsAppAccountLimit = template.WhatsAppAccountLimit,
            TotalAccountLimit = template.TotalAccountLimit,
            Amount = template.Amount,
            Currency = template.Currency,
            Plan = template.Plan,
            BillingCycle = template.BillingCycle,
            Status = template.Status,
            CurrentPeriodStartsAt = template.CurrentPeriodStartsAt,
            CurrentPeriodEndsAt = template.CurrentPeriodEndsAt
        };
    }
}