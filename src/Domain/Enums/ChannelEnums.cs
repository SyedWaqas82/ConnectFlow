namespace ConnectFlow.Domain.Enums;

public enum ChannelType
{
    WhatsApp = 1,
    Facebook = 2,
    Instagram = 3,
    Twitter = 4,
    Email = 5,
    SMS = 6,
    Custom = 7
}

public enum ChannelStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Pending = 4,
    Failed = 5
}