namespace ConnectFlow.Domain.Enums;

/// <summary>
/// Types of notes for categorization
/// </summary>
public enum NoteType
{
    General = 1,
    CallLog = 2,
    Meeting = 3,
    Email = 4,
    Task = 5,
    Reminder = 6,
    Follow = 7,
    Internal = 8,
    CustomerFeedback = 9,
    Resolution = 10
}

/// <summary>
/// Types of reactions that can be added to notes
/// </summary>
public enum ReactionType
{
    Like = 1,
    Love = 2,
    Thumbsup = 3,
    Thumbsdown = 4,
    Laugh = 5,
    Wow = 6,
    Sad = 7,
    Angry = 8
}
