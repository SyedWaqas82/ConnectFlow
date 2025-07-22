namespace ConnectFlow.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset Created { get; set; }

    public int? CreatedBy { get; set; }
    public TenantUser CreatedByUser { get; set; } = null!;

    public DateTimeOffset LastModified { get; set; }

    public int? LastModifiedBy { get; set; }
    public TenantUser LastModifiedByUser { get; set; } = null!;
}