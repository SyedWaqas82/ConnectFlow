namespace ConnectFlow.Domain.Common;

/// <summary>
/// Interface for entities that support change logging/audit trails
/// </summary>
public interface IChangeLogableEntity
{
    /// <summary>
    /// The unique identifier of the entity
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Collection of change logs for this entity
    /// </summary>
    IList<EntityChangeLog> ChangeLogs { get; set; }

    /// <summary>
    /// The type of entity for logging purposes
    /// </summary>
    EntityType EntityType { get; }

    /// <summary>
    /// List of property names that should be tracked for changes
    /// If empty or null, all properties will be tracked
    /// </summary>
    IList<string> GetLoggableFields();

    /// <summary>
    /// Gets the display name for a property (for UI purposes)
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <returns>The display name or the property name if no display name is defined</returns>
    string GetPropertyDisplayName(string propertyName);

    /// <summary>
    /// Formats a property value for display in the change log
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <param name="value">The property value</param>
    /// <returns>The formatted value for display</returns>
    string FormatValueForDisplay(string propertyName, object? value);
}