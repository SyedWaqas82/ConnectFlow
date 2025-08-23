namespace ConnectFlow.Application.Common.Exceptions;

public class TenantNotFoundException : Exception
{
    public TenantNotFoundException() : base($"Tenant not found or provided.")
    {
    }
}