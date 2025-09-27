namespace ConnectFlow.Domain.Enums;

public enum SequenceStepType
{
    Email = 1,
    Activity = 2,
}

public enum SequenceEnrollmentStatus
{
    Active = 1,
    Paused = 2,
    Completed = 3,
    Stopped = 4,
    Failed = 5
}