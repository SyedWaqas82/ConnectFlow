using ConnectFlow.Domain.Identity;

namespace ConnectFlow.Domain.Entities;

public class Lead : BaseAuditableEntity, ITenantEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Currency { get; set; } = "USD";
    public LeadStatus Status { get; set; }
    public DateTimeOffset? ExpectedCloseDate { get; set; }

    public int? ContactId { get; set; }
    public Contact? Contact { get; set; }

    public int? CompanyId { get; set; }
    public Company? Company { get; set; }

    public int? StageId { get; set; }
    public Stage? Stage { get; set; }

    public int? PipelineId { get; set; }
    public Pipeline? Pipeline { get; set; }

    public int? OwnerId { get; set; }
    public ApplicationUser? Owner { get; set; }

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
