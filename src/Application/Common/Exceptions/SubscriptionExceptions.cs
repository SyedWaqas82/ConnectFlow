namespace ConnectFlow.Application.Common.Exceptions;

public class SubscriptionRequiredException : Exception
{
    public SubscriptionRequiredException(string message = null!) : base(message ?? "This operation requires an active subscription")
    {
    }
}

public class SubscriptionLimitExceededException : Exception
{
    public string LimitType { get; }
    public int CurrentLimit { get; }
    public int CurrentUsage { get; }

    public SubscriptionLimitExceededException(string limitType, int currentLimit, int currentUsage) : base($"{limitType} limit exceeded. Current limit: {currentLimit}, Current usage: {currentUsage}")
    {
        LimitType = limitType;
        CurrentLimit = currentLimit;
        CurrentUsage = currentUsage;
    }
}

public class SubscriptionNotFoundException : Exception
{
    public SubscriptionNotFoundException(string message = null!) : base(message ?? "Subscription not found")
    {
    }
}

public class PlanNotFoundException : Exception
{
    public PlanNotFoundException(string message = null!) : base(message ?? "Plan not found")
    {
    }
}