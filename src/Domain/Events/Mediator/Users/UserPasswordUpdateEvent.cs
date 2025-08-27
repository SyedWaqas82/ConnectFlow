namespace ConnectFlow.Domain.Events.Mediator.Users;

public class UserPasswordUpdateEvent : BaseEvent
{
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    public UserPasswordUpdateEvent(int tenantId, int applicationUserId) : base(tenantId, applicationUserId)
    {
    }
}