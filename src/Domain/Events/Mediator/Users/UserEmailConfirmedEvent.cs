namespace ConnectFlow.Domain.Events.Mediator.Users;

public class UserEmailConfirmedEvent : BaseEvent
{
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}