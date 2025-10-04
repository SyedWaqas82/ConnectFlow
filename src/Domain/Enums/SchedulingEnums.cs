namespace ConnectFlow.Domain.Enums;

public enum StartingInterval
{
    OnTheTenMinutes = 1, // e.g., 1:10, 2:10
    OnTheFifteenMinutes = 2, // e.g., 1:15, 2:15
    OnTheThirtyMinutes = 3, // e.g., 1:30, 2:30
    OnTheOneHour = 4, // e.g., 1:00, 2:00
}

public enum MinimumNoticeToBook
{
    NoMinimum = 0,
    FifteenMinutes = 15,
    ThirtyMinutes = 30,
    OneHour = 60,
    TwoHours = 120,
    FourHours = 240,
    EightHours = 480,
    TwelveHours = 720,
    OneDay = 1440,
    TwoDays = 2880,
    ThreeDays = 4320,
    SevenDays = 10080,
}

public enum MaximumAdvanceToBook
{
    OneWeek = 7,
    TwoWeeks = 14,
    ThreeWeeks = 21,
    OneMonth = 30,
    TwoMonths = 60,
    ThreeMonths = 90,
    FourMonths = 120,
    FiveMonths = 150,
    SixMonths = 180,
}

public enum ScheduleDayOfWeek
{
    Sunday = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6
}

public enum SchedulingSlotType
{
    Available = 1,    // Regular available slot
    Blocked = 2,      // Blocked/unavailable slot
    Booked = 3        // Booked with activity
}

public enum FollowUpDuration
{
    OneWeek = 7,
    TwoWeeks = 14,
    ThreeWeeks = 21,
    OneMonth = 30,
    TwoMonths = 60,
    ThreeMonths = 90,
    FourMonths = 120,
    FiveMonths = 150,
    SixMonths = 180,
    SevenMonths = 210,
    EightMonths = 240,
    NineMonths = 270,
    TenMonths = 300,
    ElevenMonths = 330,
    TwelveMonths = 365
}