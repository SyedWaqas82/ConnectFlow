namespace ConnectFlow.Domain.Entities;

public class DealParticipant : BaseAuditableEntity
{
    public int DealId { get; set; }
    public Deal Deal { get; set; } = null!;

    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public bool IsPrimary { get; set; } = false;
}