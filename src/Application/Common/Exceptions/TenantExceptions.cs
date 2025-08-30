namespace ConnectFlow.Application.Common.Exceptions;

public class TenantNotFoundException : Exception
{
    public TenantNotFoundException(string message = null!) : base(message ?? "Tenant not found or provided.")
    {
    }
}