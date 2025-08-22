namespace ConnectFlow.Application.Common.Security;

/// <summary>
/// Specifies that the request requires an active subscription for the current tenant.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class AuthorizeTenantSubscriptionAttribute : Attribute
{
    public IReadOnlyList<string> Roles { get; }
    public bool AllowSuperAdmin { get; }

    public AuthorizeTenantSubscriptionAttribute(bool allowSuperAdmin = true, params string[] roles)
    {
        AllowSuperAdmin = allowSuperAdmin;
        Roles = roles ?? Array.Empty<string>();
    }
}