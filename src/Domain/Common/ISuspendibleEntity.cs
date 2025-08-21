namespace ConnectFlow.Domain.Common;

public interface ISuspendibleEntity
{
    EntityStatus EntityStatus { get; set; }
    DateTimeOffset? SuspendedAt { get; set; }
    DateTimeOffset? ResumedAt { get; set; }
}