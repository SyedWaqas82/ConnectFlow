using MediatR;

namespace ConnectFlow.Domain.Common;

public abstract class BaseEvent : INotification
{
    public int ApplicationUserId { get; }
    public Guid? ApplicationUserPublicId { get; init; }
    public int TenantId { get; }
    public Guid? CorrelationId { get; init; }
    public Guid Id { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;

    public BaseEvent(int tenantId, int applicationUserId)
    {
        TenantId = tenantId;
        ApplicationUserId = applicationUserId;
    }
}