namespace ConnectFlow.Application.Common.Security;

/// <summary>
/// Specifies that the request requires an active subscription for the current tenant.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class TenantWithActiveSubscriptionAttribute : Attribute
{
    public TenantWithActiveSubscriptionAttribute() { }

    /// <summary>
    /// Whether SuperAdmin is allowed to bypass the subscription requirement. Default is true.
    /// </summary>
    public bool AllowSuperAdmin { get; set; } = true;
}