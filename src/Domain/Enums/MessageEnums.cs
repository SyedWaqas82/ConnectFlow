namespace ConnectFlow.Domain.Enums;

public enum MessageChannel
{
    InApp = 0,
    Email = 1,
    SMS = 2,
    WhatsApp = 3,
    Custom = 4
}

public enum MessageDirection
{
    Inbound = 0,
    Outbound = 1
}

public enum MessageStatus
{
    Created = 0,
    Queued = 1,
    Sent = 2,
    Delivered = 3,
    Read = 4,
    Failed = 5,
    Spam = 6,
    Bounced = 7
}
