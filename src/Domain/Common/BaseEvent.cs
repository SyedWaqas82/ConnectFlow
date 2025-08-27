using MediatR;

namespace ConnectFlow.Domain.Common;

public abstract class BaseEvent : INotification
{
    public int? ApplicationUserId { get; init; }
    public Guid? ApplicationUserPublicId { get; init; }
    public int? TenantId { get; init; }
    public Guid? CorrelationId { get; init; }
    public Guid Id { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}