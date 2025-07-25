namespace ConnectFlow.Domain.Enums;

public enum SubscriptionPlan
{
    Free = 1,
    Starter = 2,
    Professional = 3,
    Business = 4,
    Enterprise = 5
}

public enum SubscriptionStatus
{
    Inactive = 1,
    Active = 2,
    PastDue = 3,
    Canceled = 4,
    Trialing = 5
}