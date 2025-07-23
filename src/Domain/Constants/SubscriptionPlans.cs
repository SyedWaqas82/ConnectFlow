namespace ConnectFlow.Domain.Constants;

using ConnectFlow.Domain.Entities;

public static class SubscriptionPlans
{
    public static readonly Subscription Free = new Subscription
    {
        UserLimit = 1,
        LeadLimit = 10,
        ContactLimit = 50,
        CompanyLimit = 25,
        CustomFieldLimit = 5,
        MonthlyAITokenLimit = 1000,
        Amount = 0,
        Currency = "USD",
        Plan = SubscriptionPlan.Free,
        BillingCycle = BillingCycle.Monthly,
        Status = SubscriptionStatus.Active,
        CurrentPeriodStartsAt = DateTimeOffset.UtcNow,
        CurrentPeriodEndsAt = DateTimeOffset.UtcNow.AddMonths(1),
    };

    public static readonly Subscription Starter = new Subscription
    {
        UserLimit = 5,
        LeadLimit = 100,
        ContactLimit = 500,
        CompanyLimit = 25,
        CustomFieldLimit = 25,
        MonthlyAITokenLimit = 10000,
        Amount = 29,
        Currency = "USD",
        Plan = SubscriptionPlan.Starter,
        BillingCycle = BillingCycle.Monthly,
        Status = SubscriptionStatus.Active,
        CurrentPeriodStartsAt = DateTimeOffset.UtcNow,
        CurrentPeriodEndsAt = DateTimeOffset.UtcNow.AddMonths(1),
    };

    public static readonly Subscription Professional = new Subscription
    {
        UserLimit = 25,
        LeadLimit = 1000,
        ContactLimit = 25000,
        CompanyLimit = 100,
        CustomFieldLimit = 100,
        MonthlyAITokenLimit = 100000,
        Amount = 99,
        Currency = "USD",
        Plan = SubscriptionPlan.Professional,
        BillingCycle = BillingCycle.Monthly,
        Status = SubscriptionStatus.Active,
        CurrentPeriodStartsAt = DateTimeOffset.UtcNow,
        CurrentPeriodEndsAt = DateTimeOffset.UtcNow.AddMonths(1),
    };

    public static readonly Subscription Enterprise = new Subscription
    {
        UserLimit = int.MaxValue,
        LeadLimit = int.MaxValue,
        ContactLimit = int.MaxValue,
        CompanyLimit = int.MaxValue,
        CustomFieldLimit = int.MaxValue,
        MonthlyAITokenLimit = int.MaxValue,
        Amount = 499,
        Currency = "USD",
        Plan = SubscriptionPlan.Enterprise,
        BillingCycle = BillingCycle.Monthly,
        Status = SubscriptionStatus.Active,
        CurrentPeriodStartsAt = DateTimeOffset.UtcNow,
        CurrentPeriodEndsAt = DateTimeOffset.UtcNow.AddMonths(1),
    };

    public static Subscription GetPlanByName(SubscriptionPlan plan)
    {
        return plan switch
        {
            SubscriptionPlan.Free => Free,
            SubscriptionPlan.Starter => Starter,
            SubscriptionPlan.Professional => Professional,
            SubscriptionPlan.Enterprise => Enterprise,
            _ => throw new ArgumentException($"Unknown subscription plan: {plan}")
        };
    }
}