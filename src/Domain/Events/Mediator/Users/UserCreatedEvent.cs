namespace ConnectFlow.Domain.Events.Mediator.Users;

public class UserCreatedEvent : BaseEvent
{
    public required string Email { get; init; }
    public string[]? Cc { get; init; }
    public string[]? Bcc { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? JobTitle { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Mobile { get; init; }
    public string? TimeZone { get; init; }
    public string? Locale { get; init; }
    public bool EmailConfirmed { get; init; }
    public required string ConfirmationToken { get; init; }
}