namespace ConnectFlow.Application.Common.Interfaces;

/// <summary>
/// Service to access tenant information in both HTTP and non-HTTP contexts
/// </summary>
public interface ICurrentTenantService
{
    /// <summary>
    /// Gets the current tenant ID
    /// </summary>
    int? GetCurrentTenantId();
}
