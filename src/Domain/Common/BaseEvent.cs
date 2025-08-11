using MediatR;

namespace ConnectFlow.Domain.Common;

public abstract class BaseEvent : INotification
{
    public int? ApplicationUserId { get; init; }
    public Guid? PublicUserId { get; init; }
    public int? TenantId { get; init; }
    public Guid CorrelationId { get; init; }
}