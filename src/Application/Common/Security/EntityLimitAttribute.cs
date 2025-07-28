using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Application.Common.Security;

/// <summary>
/// Specifies that the request should validate the tenant's limit for a specific entity type
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class EntityLimitAttribute : Attribute
{
    public EntityType[] EntityTypes { get; }

    public EntityLimitAttribute(EntityType[] entityTypes)
    {
        EntityTypes = entityTypes;
    }
}