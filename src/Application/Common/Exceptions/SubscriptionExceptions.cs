namespace ConnectFlow.Application.Common.Exceptions;

public class SubscriptionRequiredException : Exception
{
    public SubscriptionRequiredException() : base("This operation requires an active subscription")
    {
    }

    public SubscriptionRequiredException(string message) : base(message)
    {
    }

    public SubscriptionRequiredException(string message, Exception innerException) : base(message, innerException)
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
    public SubscriptionNotFoundException() : base("Subscription not found")
    {
    }

    public SubscriptionNotFoundException(string message) : base(message)
    {
    }

    public SubscriptionNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class PlanNotFoundException : Exception
{
    public PlanNotFoundException() : base("Plan not found")
    {
    }

    public PlanNotFoundException(string message) : base(message)
    {
    }

    public PlanNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}