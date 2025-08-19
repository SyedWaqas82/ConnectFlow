namespace ConnectFlow.Domain.Enums;

public enum ChannelType
{
    Unknown = 0,
    WhatsApp = 1,
    Facebook = 2,
    Instagram = 3,
    Telegram = 4,
}

public enum ChannelStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Pending = 4,
    Failed = 5
}