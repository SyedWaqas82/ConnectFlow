using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Domain.Entities;

public class Person : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity, ILabelableEntity, IActivatableEntity, INoteableEntity, IFileableEntity, IDocumentableEntity, IChangeLogableEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int? OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public int OwnerId { get; set; }
    public TenantUser Owner { get; set; } = null!;
    public IList<PersonPhone> Phones { get; private set; } = new List<PersonPhone>();
    public IList<PersonEmail> Emails { get; private set; } = new List<PersonEmail>();
    public IList<EntityActivityParticipant> ParticipatingActivities { get; private set; } = new List<EntityActivityParticipant>();
    public IList<EntityParticipant> Participants { get; private set; } = new List<EntityParticipant>();
    public IList<Lead> Leads { get; private set; } = new List<Lead>();
    public IList<Deal> Deals { get; private set; } = new List<Deal>();
    public IList<Project> Projects { get; private set; } = new List<Project>();

    [NotMapped]
    public EntityType EntityType => EntityType.Person;
    [NotMapped]
    public IList<EntityLabel> Labels { get; set; } = new List<EntityLabel>();
    [NotMapped]
    public IList<EntityActivity> Activities { get; set; } = new List<EntityActivity>();
    [NotMapped]
    public IList<EntityNote> Notes { get; set; } = new List<EntityNote>();
    [NotMapped]
    public IList<EntityFile> Files { get; set; } = new List<EntityFile>();
    [NotMapped]
    public IList<EntityDocument> Documents { get; set; } = new List<EntityDocument>();
    [NotMapped]
    public IList<EntityChangeLog> ChangeLogs { get; set; } = new List<EntityChangeLog>();

    // ITenantEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ISoftDeleteEntity implementation
    public bool IsDeleted { get; set; } = false; // Soft delete flag
    public DateTimeOffset? DeletedAt { get; set; } = null!; // When the entity was deleted
    public int? DeletedBy { get; set; } = null!; // User who deleted the entity

    // ISuspendibleEntity implementation
    public EntityStatus EntityStatus { get; set; } = EntityStatus.Active; // Overall status of the entity
    public DateTimeOffset? SuspendedAt { get; set; }
    public DateTimeOffset? ResumedAt { get; set; }

    // IChangeLogableEntity implementation
    public string FormatValueForDisplay(string propertyName, object? value)
    {
        if (value == null) return "Not Set";

        return propertyName switch
        {
            nameof(IsDeleted) when value is bool b => b ? "Yes" : "No",
            nameof(EntityStatus) when value is EntityStatus b => b == EntityStatus.Suspended ? "Yes" : "No",
            _ => value.ToString() ?? "Not Set"
        };
    }

    public IList<string> GetLoggableFields()
    {
        return new List<string>
        {
            nameof(FirstName),
            nameof(LastName),
            nameof(OrganizationId),
            nameof(OwnerId),
            nameof(IsDeleted),
            nameof(EntityStatus)
        };
    }

    public string GetPropertyDisplayName(string propertyName)
    {
        return propertyName switch
        {
            nameof(FirstName) => "First Name",
            nameof(LastName) => "Last Name",
            nameof(OrganizationId) => "Organization",
            nameof(OwnerId) => "Owner",
            nameof(IsDeleted) => "Deleted",
            nameof(EntityStatus) => "Suspended",
            _ => propertyName
        };
    }
}