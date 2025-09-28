namespace ConnectFlow.Domain.Entities;

public class DealStageHistory : BaseAuditableEntity, ITenantableEntity
{
    public int DealId { get; set; }
    public Deal Deal { get; set; } = null!;
    public int PipelineStageId { get; set; }
    public PipelineStage PipelineStage { get; set; } = null!;
    public int PipelineId { get; set; }
    public Pipeline Pipeline { get; set; } = null!;
    public DateTimeOffset EnteredAt { get; set; }
    public DateTimeOffset? ExitedAt { get; set; }
    public int? PreviousStageId { get; set; }
    public PipelineStage? PreviousStage { get; set; } = null!;
    public int? NextStageId { get; set; }
    public PipelineStage? NextStage { get; set; } = null!;
    public bool IsCurrentStage { get; set; } = false;

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}