using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Domain.Entities;

public class Deal : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity, IParticipantable, IActivatableEntity, ILabelableEntity, INoteableEntity, IFileableEntity, IDocumentableEntity, IChangeLogableEntity, ISequenceableEntity
{
    public required string Title { get; set; }
    public int OwnerId { get; set; }
    public TenantUser Owner { get; set; } = null!;
    public int? PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; } = null!;
    public int? LeadId { get; set; } // Optional association to a Lead if the Deal was created from a Lead
    public Lead Lead { get; set; } = null!; // Navigation property for the associated Lead
    public bool IsArchived { get; set; }
    public decimal? Value { get; set; }
    public string Currency { get; set; } = "USD";
    public TaxType TaxType { get; set; } = TaxType.None;
    public int Probability { get; set; } = 0;
    public int Score { get; set; } = 0;
    public decimal? ScorePercentage { get; set; }
    public DateTimeOffset? LastScoredAt { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public string? SourceOrigin { get; set; }
    public LeadSourceChannel SourceChannel { get; set; } = LeadSourceChannel.None;
    public string? SourceChannelId { get; set; }
    public int? PipelineStageId { get; set; }
    public PipelineStage PipelineStage { get; set; } = null!;
    public int PipelineId { get; set; }
    public Pipeline Pipeline { get; set; } = null!;
    public DealStatus Status { get; set; } = DealStatus.Open;
    public string WonLossReason { get; set; } = string.Empty;
    public IList<DealProduct> Products { get; private set; } = new List<DealProduct>();
    public IList<ProjectDeal> ProjectDeals { get; private set; } = new List<ProjectDeal>();
    public IList<DealInstallment> Installments { get; private set; } = new List<DealInstallment>();
    public IList<DealStageHistory> StageHistories { get; private set; } = new List<DealStageHistory>();

    [NotMapped]
    public EntityType EntityType => EntityType.Deal;
    [NotMapped]
    public IList<EntityParticipant> Participants { get; set; } = new List<EntityParticipant>();
    [NotMapped]
    public IList<EntityActivity> Activities { get; set; } = new List<EntityActivity>();
    [NotMapped]
    public IList<EntityLabel> Labels { get; set; } = new List<EntityLabel>();
    [NotMapped]
    public IList<EntityNote> Notes { get; set; } = new List<EntityNote>();
    [NotMapped]
    public IList<EntityFile> Files { get; set; } = new List<EntityFile>();
    [NotMapped]
    public IList<EntityDocument> Documents { get; set; } = new List<EntityDocument>();
    [NotMapped]
    public IList<EntityChangeLog> ChangeLogs { get; set; } = new List<EntityChangeLog>();
    [NotMapped]
    public IList<EntitySequenceEnrollment> SequenceEnrollments { get; set; } = new List<EntitySequenceEnrollment>();

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
            nameof(Value) when value is decimal val => $"${val:N2}",
            nameof(Score) when value is int score => score.ToString(),
            nameof(ScorePercentage) when value is decimal percentage => $"{percentage:F1}%",
            nameof(IsDeleted) when value is bool b => b ? "Yes" : "No",
            nameof(EntityStatus) when value is EntityStatus b => b == EntityStatus.Suspended ? "Yes" : "No",
            nameof(Status) when value is DealStatus status => status.ToString(),
            _ => value.ToString() ?? "Not Set"
        };
    }

    public IList<string> GetLoggableFields()
    {
        return new List<string>
        {
            nameof(Title),
            nameof(Value),
            nameof(Currency),
            nameof(Score),
            nameof(ScorePercentage),
            nameof(OwnerId),
            nameof(PersonId),
            nameof(OrganizationId),
            nameof(PipelineStageId),
            nameof(Status),
            nameof(IsDeleted),
            nameof(EntityStatus)
        };
    }

    public string GetPropertyDisplayName(string propertyName)
    {
        return propertyName switch
        {
            nameof(Title) => "Deal Title",
            nameof(Value) => "Deal Value",
            nameof(Currency) => "Currency",
            nameof(Score) => "Deal Score",
            nameof(ScorePercentage) => "Score Percentage",
            nameof(OwnerId) => "Owner",
            nameof(PersonId) => "Contact Person",
            nameof(OrganizationId) => "Organization",
            nameof(PipelineStageId) => "Pipeline Stage",
            nameof(Status) => "Deal Status",
            nameof(IsDeleted) => "Deleted",
            nameof(EntityStatus) => "Suspended",
            _ => propertyName
        };
    }
}