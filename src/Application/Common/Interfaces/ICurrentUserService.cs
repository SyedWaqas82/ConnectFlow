namespace ConnectFlow.Application.Common.Interfaces;

/// <summary>
/// Service to access current user information in both HTTP and non-HTTP contexts
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's public ID (Guid)
    /// </summary>
    Guid? GetCurrentUserId();

    /// <summary>
    /// Gets the current user's application ID (internal integer ID)
    /// </summary>
    int? GetCurrentApplicationUserId();

    /// <summary>
    /// Gets the current user's username
    /// </summary>
    string? GetCurrentUserName();

    /// <summary>
    /// Gets the current user's roles
    /// </summary>
    List<string> GetCurrentUserRoles();

    /// <summary>
    /// Checks if the current user is a super admin
    /// </summary>
    bool IsSuperAdmin();
}