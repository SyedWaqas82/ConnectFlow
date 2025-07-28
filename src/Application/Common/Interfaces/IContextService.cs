namespace ConnectFlow.Application.Common.Interfaces;

/// <summary>
/// Unified service for managing user and tenant context across the application
/// This ensures consistent context handling in both HTTP and non-HTTP scenarios
/// </summary>
public interface IContextService
{
    /// <summary>
    /// Initialize context for a specific user ID and tenant
    /// </summary>
    Task InitializeContextByUserWithTenantAsync(int applicationUserId, int? tenantId);

    /// <summary>
    /// Initialize context for a specific user with their default tenant
    /// </summary>
    Task InitializeContextForUserWithDefaultTenantAsync(int applicationUserId);

    /// <summary>
    /// Get the current tenant ID in this context
    /// </summary>
    int? GetCurrentTenantId();

    /// <summary>
    /// Get the current public user ID in this context
    /// </summary>
    Guid? GetCurrentPublicUserId();

    /// <summary>
    /// Get the current application user ID in this context
    /// </summary>
    int? GetCurrentApplicationUserId();

    /// <summary>
    /// Get the current user name in this context
    /// </summary>
    string? GetCurrentUserName();

    /// <summary>
    /// Check if the current context has SuperAdmin privileges
    /// </summary>
    bool IsSuperAdmin();

    /// <summary>
    /// Get the roles for the current user in this context
    /// </summary>
    IList<string> GetCurrentUserRoles();

    /// <summary>
    /// Check if the current user has a specific role
    /// </summary>
    bool IsInRole(string role);

    /// <summary>
    /// Clear all context information
    /// </summary>
    Task ClearContextAsync();
}