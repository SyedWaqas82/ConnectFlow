using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Application.Common.Security;

/// <summary>
/// Specifies that the request should validate the tenant's limit for one or more entity types.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ValidateLimitsAttribute : Attribute
{
    public IReadOnlyList<EntityType> EntityTypes { get; }

    public ValidateLimitsAttribute(params EntityType[] entityTypes)
    {
        EntityTypes = entityTypes ?? Array.Empty<EntityType>();
    }
}