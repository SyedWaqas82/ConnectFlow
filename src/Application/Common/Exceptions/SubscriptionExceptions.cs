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