namespace ConnectFlow.Application.Common.Exceptions;

public class EntityLimitExceededException : Exception
{
    public EntityLimitExceededException() : base("You have reached the limit for this entity type in your current subscription plan")
    {
    }

    public EntityLimitExceededException(string message) : base(message)
    {
    }

    public EntityLimitExceededException(string message, Exception innerException) : base(message, innerException)
    {
    }
}