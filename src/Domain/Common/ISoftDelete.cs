namespace ConnectFlow.Domain.Common;

/// <summary>
/// Interface for entities that support soft delete functionality
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Indicates whether this entity has been soft-deleted
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// The timestamp when this entity was deleted, null if not deleted
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// The ID of the user who deleted this entity, null if not deleted
    /// </summary>
    int? DeletedBy { get; set; }
}