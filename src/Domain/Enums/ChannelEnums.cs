namespace ConnectFlow.Domain.Enums;

public enum ChannelType
{
    WhatsApp = 0,
    Facebook = 1,
    Instagram = 2,
    Twitter = 3,
    Email = 4,
    SMS = 5,
    Custom = 6
}

public enum ChannelStatus
{
    Active = 0,
    Inactive = 1,
    Suspended = 2,
    Pending = 3,
    Failed = 4
}
