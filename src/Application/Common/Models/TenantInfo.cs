namespace ConnectFlow.Application.Common.Services;

public class TenantInfo
{
    private static readonly AsyncLocal<int?> _currentTenantId = new AsyncLocal<int?>();
    private static readonly AsyncLocal<bool> _isSuperAdmin = new AsyncLocal<bool>();

    /// <summary>
    /// Gets or sets the current tenant ID using AsyncLocal for thread safety
    /// This works in both HTTP and non-HTTP contexts
    /// </summary>
    public static int? CurrentTenantId
    {
        get => _currentTenantId.Value;
        set => _currentTenantId.Value = value;
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
}