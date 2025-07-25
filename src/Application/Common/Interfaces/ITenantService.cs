using ConnectFlow.Domain.Entities;

namespace ConnectFlow.Application.Common.Interfaces;

public interface ITenantService
{
    /// <summary>
    /// Gets the current tenant ID from the context
    /// </summary>
    /// <returns>The current tenant ID or null if no tenant is set</returns>
    Task<int?> GetCurrentTenantIdAsync();

    /// <summary>
    /// Gets the current tenant entity from the context
    /// </summary>
    /// <returns>The current tenant or null if no tenant is set</returns>
    Task<Tenant?> GetCurrentTenantAsync();

    /// <summary>
    /// Sets the current tenant ID for the current context
    /// </summary>
    /// <param name="tenantId">The tenant ID to set</param>
    Task SetCurrentTenantIdAsync(int tenantId);

    /// <summary>
    /// Clears the current tenant ID from the context
    /// </summary>
    Task ClearCurrentTenantAsync();

    /// <summary>
    /// Checks if the current user is a Super Admin
    /// </summary>
    /// <returns>True if the current user is a Super Admin, false otherwise</returns>
    bool IsSuperAdmin();
}