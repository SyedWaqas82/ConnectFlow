namespace ConnectFlow.Domain.Common;

public abstract class MessageBaseEvent
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public string CorrelationId { get; init; } = string.Empty;
    public int? TenantId { get; init; }
    public int? UserId { get; init; }
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