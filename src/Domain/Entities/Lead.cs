using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Domain.Entities;

public class Lead : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity, IActivatableEntity, ILabelableEntity, INoteableEntity
{
    public required string Title { get; set; }
    public int OwnerId { get; set; }
    public TenantUser Owner { get; set; } = null!;
    public int? PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; } = null!;
    public int? DealId { get; set; }
    public Deal Deal { get; set; } = null!;
    public bool IsArchived { get; set; }
    public decimal? Value { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime? ExpectedCloseDate { get; set; }
    public string? SourceOrigin { get; set; }
    public LeadSourceChannel SourceChannel { get; set; } = LeadSourceChannel.None;
    public string? SourceChannelId { get; set; }

    [NotMapped]
    public EntityType EntityType => EntityType.Lead;
    [NotMapped]
    public IList<EntityActivity> Activities { get; set; } = new List<EntityActivity>();
    [NotMapped]
    public IList<EntityLabel> Labels { get; set; } = new List<EntityLabel>();
    [NotMapped]
    public IList<Note> Notes { get; set; } = new List<Note>();

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
}