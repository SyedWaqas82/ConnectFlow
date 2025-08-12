namespace ConnectFlow.Application.Common.Interfaces;

/// <summary>
/// Comprehensive context service that combines user and tenant operations
/// </summary>
public interface IContextManager
{
    /// <summary>
    /// Initialize context with user and tenant information
    /// </summary>
    Task InitializeContextAsync(int applicationUserId, int? tenantId);

    /// <summary>
    /// Initialize context with default tenant for a user
    /// </summary>
    Task InitializeContextWithDefaultTenantAsync(int applicationUserId);

    /// <summary>
    /// Manually set context for background jobs or non-HTTP contexts
    /// </summary>
    void SetContext(int? applicationUserId, Guid? publicUserId, string? userName, List<string>? roles, bool isSuperAdmin, int? tenantId);

    /// <summary>
    /// Clear all context information
    /// </summary>
    void ClearContext();

    /// <summary>
    /// Gets the current user's application ID (internal integer ID)
    /// </summary>
    int? GetCurrentApplicationUserId();

    /// <summary>
    /// Gets the current user's public ID (Guid)
    /// </summary>
    Guid? GetCurrentUserId();

    /// <summary>
    /// Gets the current user's username
    /// </summary>
    string? GetCurrentUserName();

    /// <summary>
    /// Gets the current user's roles
    /// </summary>
    List<string> GetCurrentUserRoles();

    /// <summary>
    /// Gets the current tenant ID
    /// </summary>
    int? GetCurrentTenantId();

    /// <summary>
    /// Checks if the current user is a super admin
    /// </summary>
    bool IsSuperAdmin();

    /// <summary>
    /// Check if the current user has a specific role
    /// </summary>
    bool IsInRole(string role);
}