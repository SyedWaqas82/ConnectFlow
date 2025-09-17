namespace ConnectFlow.Domain.Entities;

public class EntityActivityParticipant : BaseAuditableEntity
{
    public int ActivityId { get; set; }
    public EntityActivity Activity { get; set; } = null!;

    public int? PersonId { get; set; }
    public Person Person { get; set; } = null!;

    // For Guests
    public string? GuestEmail { get; set; }
    public string? GuestName { get; set; }
    public string? GuestPhoneNumber { get; set; }
}