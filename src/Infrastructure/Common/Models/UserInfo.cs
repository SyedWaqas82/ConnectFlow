namespace ConnectFlow.Infrastructure.Common.Models;

public class UserInfo
{
    private static readonly AsyncLocal<int?> _applicationUserId = new AsyncLocal<int?>();
    private static readonly AsyncLocal<Guid?> _publicUserId = new AsyncLocal<Guid?>();
    private static readonly AsyncLocal<string?> _userName = new AsyncLocal<string?>();
    private static readonly AsyncLocal<List<string>> _roles = new AsyncLocal<List<string>>();
    private static readonly AsyncLocal<bool> _isSuperAdmin = new AsyncLocal<bool>();

    /// <summary>
    /// Gets or sets the current application user ID using AsyncLocal for thread safety
    /// This works in both HTTP and non-HTTP contexts
    /// </summary>
    public static int? ApplicationUserId
    {
        get => _applicationUserId.Value;
        set => _applicationUserId.Value = value;
    }

    /// <summary>
    /// Gets or sets the current public user ID using AsyncLocal for thread safety
    /// </summary>
    public static Guid? PublicUserId
    {
        get => _publicUserId.Value;
        set => _publicUserId.Value = value;
    }

    /// <summary>
    /// Gets or sets the current user name using AsyncLocal for thread safety
    /// </summary>
    public static string? UserName
    {
        get => _userName.Value;
        set => _userName.Value = value;
    }

    /// <summary>
    /// Gets or sets the current user roles using AsyncLocal for thread safety
    /// </summary>
    public static List<string> Roles
    {
        get => _roles.Value ?? new List<string>();
        set => _roles.Value = value;
    }

    /// <summary>
    /// Gets or sets whether the current user is a SuperAdmin
    /// This is used to bypass tenant filtering for SuperAdmin users
    /// </summary>
    public static bool IsSuperAdmin
    {
        get => _isSuperAdmin.Value;
        set => _isSuperAdmin.Value = value;
    }

    /// <summary>
    /// Check if the current user has a specific role
    /// </summary>
    public static bool IsInRole(string role)
    {
        return Roles.Contains(role);
    }

    /// <summary>
    /// Clear all user context information
    /// </summary>
    public static void Clear()
    {
        ApplicationUserId = null;
        PublicUserId = null;
        UserName = null;
        Roles = new List<string>();
        IsSuperAdmin = false;
    }
}