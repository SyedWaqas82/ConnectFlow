namespace ConnectFlow.Application.Common.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class TenantRoleAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenantRoleAttribute"/> class with the specified roles.
    /// </summary>
    public TenantRoleAttribute() { }

    /// <summary>
    /// Gets the comma-delimited list of roles that are allowed to access the resource.
    /// </summary>
    public string Roles { get; set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether SuperAdmin is allowed.
    /// </summary>
    public bool AllowSuperAdmin { get; set; } = true;
}