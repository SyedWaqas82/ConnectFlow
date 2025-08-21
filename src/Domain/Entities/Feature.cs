namespace ConnectFlow.Domain.Entities;

public class Feature : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public IList<Plan> Plans { get; private set; } = new List<Plan>();
}