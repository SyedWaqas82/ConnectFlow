namespace ConnectFlow.Application.Common.Security;

/// <summary>
/// Specifies that the request requires an active subscription for the current tenant
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class SubscriptionAttribute : Attribute
{
}