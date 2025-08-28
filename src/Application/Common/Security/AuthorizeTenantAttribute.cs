namespace ConnectFlow.Application.Common.Security;

/// <summary>
/// Attribute to specify that a request requires tenant authorization, with optional role and subscription checks.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class AuthorizeTenantAttribute : Attribute
{
    public IReadOnlyList<string> Roles { get; }
    public bool AllowSuperAdmin { get; }
    public bool CheckActiveSubscription { get; }

    public AuthorizeTenantAttribute(bool allowSuperAdmin = true, bool checkActiveSubscription = true, params string[] roles)
    {
        Roles = roles ?? Array.Empty<string>();
        AllowSuperAdmin = allowSuperAdmin;
        CheckActiveSubscription = checkActiveSubscription;
    }
}