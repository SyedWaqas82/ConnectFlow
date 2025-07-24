namespace ConnectFlow.Domain.Events.Users;

public class UserEmailConfirmedEvent : BaseEvent
{
    public int UserId { get; }
    public Guid PublicId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string? JobTitle { get; }
    public string? PhoneNumber { get; }
    public string? Mobile { get; }
    public string? TimeZone { get; }
    public string? Locale { get; }
    public bool EmailConfirmed { get; }
    public string ConfirmationToken { get; }

    public UserEmailConfirmedEvent(int userId, Guid publicId, string email, string firstName, string lastName, string? jobTitle, string? phoneNumber, string? mobile, string? timeZone, string? locale, bool emailConfirmed, string confirmationToken)
    {
        UserId = userId;
        PublicId = publicId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        JobTitle = jobTitle;
        PhoneNumber = phoneNumber;
        Mobile = mobile;
        TimeZone = timeZone;
        Locale = locale;
        EmailConfirmed = emailConfirmed;
        ConfirmationToken = confirmationToken;
    }
}