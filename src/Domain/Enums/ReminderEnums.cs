namespace ConnectFlow.Domain.Enums;

public enum ReminderType
{
    WorkTask = 1,
    Meeting = 2,
    Call = 3,
    Email = 4,
    Follow_Up = 5,
    Custom = 6
}

public enum ReminderChannel
{
    InApp = 1,
    Email = 2,
    SMS = 3,
    WhatsApp = 4,
    Push = 5
}
