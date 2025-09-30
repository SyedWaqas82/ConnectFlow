using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Domain.Entities;

public class Organization : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity, ILabelableEntity, IActivatableEntity, INoteableEntity, IFileableEntity, IDocumentableEntity, IChangeLogableEntity
{
    public required string Name { get; set; }
    public string? Address { get; set; }
    public string? Website { get; set; }
    public string? LinkedIn { get; set; }
    public string? Industry { get; set; }
    public int? NumberOfEmployees { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public int OwnerId { get; set; }
    public TenantUser Owner { get; set; } = null!;
    public IList<Deal> Deals { get; private set; } = new List<Deal>();
    public IList<Person> People { get; private set; } = new List<Person>();
    public IList<Lead> Leads { get; private set; } = new List<Lead>();
    public IList<Project> Projects { get; private set; } = new List<Project>();
    public IList<OrganizationRelationship> PrimaryRelationships { get; private set; } = new List<OrganizationRelationship>();
    public IList<OrganizationRelationship> RelatedRelationships { get; private set; } = new List<OrganizationRelationship>();

    [NotMapped]
    public EntityType EntityType => EntityType.Organization;
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

    public string FormatValueForDisplay(string propertyName, object? value)
    {
        if (value == null) return "Not Set";

        return propertyName switch
        {
            nameof(AnnualRevenue) when value is decimal val => $"${val:N2}",
            nameof(NumberOfEmployees) when value is int num => num.ToString(),
            nameof(IsDeleted) when value is bool b => b ? "Yes" : "No",
            nameof(EntityStatus) when value is EntityStatus b => b == EntityStatus.Suspended ? "Yes" : "No",
            _ => value.ToString() ?? "Not Set"
        };
    }

    public IList<string> GetLoggableFields()
    {
        return new List<string>
        {
            nameof(Name),
            nameof(Address),
            nameof(Website),
            nameof(LinkedIn),
            nameof(Industry),
            nameof(NumberOfEmployees),
            nameof(AnnualRevenue),
            nameof(OwnerId),
            nameof(IsDeleted),
            nameof(EntityStatus)
        };
    }

    public string GetPropertyDisplayName(string propertyName)
    {
        return propertyName switch
        {
            nameof(Name) => "Organization Name",
            nameof(Address) => "Address",
            nameof(Website) => "Website",
            nameof(LinkedIn) => "LinkedIn",
            nameof(Industry) => "Industry",
            nameof(NumberOfEmployees) => "Number of Employees",
            nameof(AnnualRevenue) => "Annual Revenue",
            nameof(OwnerId) => "Owner",
            nameof(IsDeleted) => "Deleted",
            nameof(EntityStatus) => "Status",
            _ => propertyName
        };
    }
}