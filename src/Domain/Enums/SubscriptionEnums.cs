namespace ConnectFlow.Domain.Enums;

public enum SubscriptionStatus
{
    Active = 1,
    Canceled = 2,
    PastDue = 3,
    Unpaid = 4,
    Trialing = 5,
    Incomplete = 6,
    IncompleteExpired = 7
}

public enum PlanType
{
    Free = 1,
    Starter = 2,
    Basic = 3,
    Pro = 4,
    Enterprise = 5
}