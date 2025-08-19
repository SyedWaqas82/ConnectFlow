using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Application.Common.Security;

/// <summary>
/// Specifies that the request should validate the tenant's limit for one or more entity types.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ValidateLimitsAttribute : Attribute
{
    public IReadOnlyList<LimitValidationType> LimitValidationTypes { get; }

    public ValidateLimitsAttribute(params LimitValidationType[] limitValidationTypes)
    {
        LimitValidationTypes = limitValidationTypes ?? Array.Empty<LimitValidationType>();
    }
}