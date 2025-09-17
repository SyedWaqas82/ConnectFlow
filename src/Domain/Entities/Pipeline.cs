namespace ConnectFlow.Domain.Entities;

public class Pipeline : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ISuspendibleEntity
{
    public required string Name { get; set; }
    public int SortOrder { get; set; }
    public bool DealsProbabilityEnabled { get; set; } = false;
    public IList<Deal> Deals { get; private set; } = new List<Deal>();
    public IList<PipelineStage> Stages { get; private set; } = new List<PipelineStage>();

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