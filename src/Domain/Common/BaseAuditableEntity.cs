namespace ConnectFlow.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

    public int? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.UtcNow;

    public int? LastModifiedBy { get; set; }
}