namespace ConnectFlow.Domain.Enums;

public enum ActivityType
{
    Call = 1,
    Meeting = 2,
    Task = 3,
    Deadline = 4,
    Email = 5,
    Lunch = 6,
}

public enum ActivityPriority
{
    None = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

public enum CalendarVisibilityStatus
{
    Free = 1,
    Busy = 2,
}