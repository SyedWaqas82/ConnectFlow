namespace ConnectFlow.Application.Common.Services;

public class TenantInfo
{
    private static readonly AsyncLocal<int?> _currentTenantId = new AsyncLocal<int?>();

    /// <summary>
    /// Gets or sets the current tenant ID using AsyncLocal for thread safety
    /// This works in both HTTP and non-HTTP contexts
    /// </summary>
    public static int? CurrentTenantId
    {
        get => _currentTenantId.Value;
        set => _currentTenantId.Value = value;
    }
}
