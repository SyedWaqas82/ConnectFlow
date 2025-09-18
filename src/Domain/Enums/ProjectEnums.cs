namespace ConnectFlow.Domain.Enums;

public enum ProjectStatus
{
    Planning = 1,
    Active = 2,
    OnHold = 3,
    Completed = 4,
    Cancelled = 5,
    Archived = 6
}

public enum ProjectPriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Critical = 4
}

public enum ProjectTaskStatus
{
    NotStarted = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    OnHold = 5
}