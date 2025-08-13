namespace ConnectFlow.Domain.Events.Mediator.Users;

public class UserPasswordResetEvent : BaseEvent
{
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string ResetPasswordToken { get; init; }
}