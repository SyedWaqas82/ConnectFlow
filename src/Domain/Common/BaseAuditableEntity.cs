using ConnectFlow.Domain.Identity;

namespace ConnectFlow.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset Created { get; set; }

    public int? CreatedBy { get; set; }
    public ApplicationUser CreatedByUser { get; set; } = null!;

    public DateTimeOffset LastModified { get; set; }

    public int? LastModifiedBy { get; set; }
    public ApplicationUser LastModifiedByUser { get; set; } = null!;
}