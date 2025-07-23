namespace ConnectFlow.Domain.Constants;

public abstract class Roles
{
    public const string SuperAdmin = nameof(SuperAdmin);
    public const string TenantAdmin = nameof(TenantAdmin);
    public const string NonTenantAdmin = nameof(NonTenantAdmin);

    public static readonly string[] AllRoles = { SuperAdmin, TenantAdmin, NonTenantAdmin };

    public static readonly Dictionary<string, string[]> RoleHierarchy = new()
    {
        { SuperAdmin, new[] { SuperAdmin, TenantAdmin, NonTenantAdmin } },
        { TenantAdmin, new[] { TenantAdmin, NonTenantAdmin } },
        { NonTenantAdmin, new[] { NonTenantAdmin } }
    };
}