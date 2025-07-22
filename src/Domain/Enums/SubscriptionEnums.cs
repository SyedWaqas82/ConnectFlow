namespace ConnectFlow.Domain.Enums;

public enum SubscriptionPlan
{
    Free = 0,
    Starter = 1,
    Professional = 2,
    Business = 3,
    Enterprise = 4
}

public enum SubscriptionStatus
{
    Inactive = 0,
    Active = 1,
    PastDue = 2,
    Canceled = 3,
    Trialing = 4
}
