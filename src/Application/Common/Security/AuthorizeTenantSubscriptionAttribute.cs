namespace ConnectFlow.Application.Common.Security;

/// <summary>
/// Specifies that the request requires an active subscription for the current tenant.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class AuthorizeTenantSubscriptionAttribute : Attribute
{
    public AuthorizeTenantSubscriptionAttribute() { }

    /// <summary>
    /// Gets the comma-delimited list of roles that are allowed to access the resource.
    /// </summary>
    public string Roles { get; set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether SuperAdmin is allowed.
    /// </summary>
    public bool AllowSuperAdmin { get; set; } = true;
}