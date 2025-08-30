namespace ConnectFlow.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message = null!) : base(message ?? "Forbidden access") { }
}