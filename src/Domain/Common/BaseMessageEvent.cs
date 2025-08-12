namespace ConnectFlow.Domain.Common;

public abstract class BaseMessageEvent
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public Guid? CorrelationId { get; init; } = Guid.NewGuid();
    public int? ApplicationUserId { get; init; }
    public Guid? PublicUserId { get; init; }
    public int? TenantId { get; init; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int RetryCount { get; set; }
    public Dictionary<string, object> Headers { get; init; } = new();
    public abstract string MessageType { get; }
    public void Acknowledge() => IsAcknowledged = true;
    public void Reject(bool requeue = false)
    {
        IsRejected = true;
        Requeue = requeue;
    }

    public bool IsAcknowledged { get; private set; }
    public bool IsRejected { get; private set; }
    public bool Requeue { get; private set; }
}