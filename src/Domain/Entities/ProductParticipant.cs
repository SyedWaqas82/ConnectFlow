namespace ConnectFlow.Domain.Entities;

public class ProductParticipant : BaseAuditableEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
}