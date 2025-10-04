namespace ConnectFlow.Domain.Events.Mediator.Users;

public class UserCreatedEvent : BaseEvent
{
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? JobTitle { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Mobile { get; init; }
    public string? TimeZone { get; init; }
    public string? Language { get; init; }
    public string? DateNumberFormat { get; set; }
    public string? DefaultCurrency { get; set; }
    public bool EmailConfirmed { get; init; }
    public required string ConfirmationToken { get; init; }

    public UserCreatedEvent(int tenantId, int applicationUserId) : base(tenantId, applicationUserId)
    {
    }
}