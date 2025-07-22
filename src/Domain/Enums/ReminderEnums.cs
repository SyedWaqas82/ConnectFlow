namespace ConnectFlow.Domain.Enums;

public enum ReminderType
{
    Task = 0,
    Meeting = 1,
    Call = 2,
    Email = 3,
    Follow_Up = 4,
    Custom = 5
}

public enum ReminderChannel
{
    InApp = 0,
    Email = 1,
    SMS = 2,
    WhatsApp = 3,
    Push = 4
}
