namespace ConnectFlow.Domain.Enums;

/// <summary>
/// Represents the type of scoring condition
/// </summary>
public enum ConditionType
{
    /// <summary>
    /// Condition is based on entity properties (e.g., deal value, pipeline stage)
    /// </summary>
    Property = 1,

    /// <summary>
    /// Condition is based on activities (e.g., emails sent, calls made)
    /// </summary>
    Activity = 2,

    /// <summary>
    /// Condition is based on custom fields
    /// </summary>
    CustomField = 3,

    /// <summary>
    /// Condition is based on time-based criteria (e.g., days since creation)
    /// </summary>
    TimeBased = 4,

    /// <summary>
    /// Condition is based on relationships (e.g., organization size)
    /// </summary>
    Relationship = 5
}

/// <summary>
/// Represents the comparison operator for scoring conditions
/// </summary>
public enum RuleOperator
{
    /// <summary>
    /// Equal to
    /// </summary>
    Equals = 1,

    /// <summary>
    /// Not equal to
    /// </summary>
    NotEquals = 2,

    /// <summary>
    /// Greater than
    /// </summary>
    GreaterThan = 3,

    /// <summary>
    /// Greater than or equal to
    /// </summary>
    GreaterThanOrEqual = 4,

    /// <summary>
    /// Less than
    /// </summary>
    LessThan = 5,

    /// <summary>
    /// Less than or equal to
    /// </summary>
    LessThanOrEqual = 6,

    /// <summary>
    /// Contains (for string values)
    /// </summary>
    Contains = 7,

    /// <summary>
    /// Does not contain (for string values)
    /// </summary>
    DoesNotContain = 8,

    /// <summary>
    /// Starts with (for string values)
    /// </summary>
    StartsWith = 9,

    /// <summary>
    /// Ends with (for string values)
    /// </summary>
    EndsWith = 10,

    /// <summary>
    /// Is empty or null
    /// </summary>
    IsEmpty = 11,

    /// <summary>
    /// Is not empty or null
    /// </summary>
    IsNotEmpty = 12,

    /// <summary>
    /// In a list of values
    /// </summary>
    In = 13,

    /// <summary>
    /// Not in a list of values
    /// </summary>
    NotIn = 14
}

public enum LogicalOperator
{
    And = 1,
    Or = 2
}