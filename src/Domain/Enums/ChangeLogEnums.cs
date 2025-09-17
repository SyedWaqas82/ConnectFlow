namespace ConnectFlow.Domain.Enums;

/// <summary>
/// Represents the type of change that occurred on an entity
/// </summary>
public enum ChangeType
{
    /// <summary>
    /// Entity was created
    /// </summary>
    Created = 1,

    /// <summary>
    /// Entity properties were updated
    /// </summary>
    Updated = 2,

    /// <summary>
    /// Entity was soft deleted
    /// </summary>
    Deleted = 3,

    /// <summary>
    /// Entity was restored from soft delete
    /// </summary>
    Restored = 4,

    /// <summary>
    /// Entity status was changed (activated/deactivated/suspended)
    /// </summary>
    StatusChanged = 5,

    /// <summary>
    /// Entity ownership or tenant was changed
    /// </summary>
    OwnershipChanged = 6,

    /// <summary>
    /// Custom business operation
    /// </summary>
    Custom = 99
}