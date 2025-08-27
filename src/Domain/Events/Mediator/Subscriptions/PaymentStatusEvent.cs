namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

/// <summary>
/// Handles payment-related events and their consequences
/// </summary>
public class PaymentStatusEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public PaymentAction Action { get; }
    public string Reason { get; }
    public decimal? Amount { get; }
    public bool SendEmailNotification { get; }
    public int FailureCount { get; }
    public DateTimeOffset Timestamp { get; }

    public PaymentStatusEvent(
        int subscriptionId,
        PaymentAction action,
        string reason,
        decimal? amount = null,
        bool sendEmailNotification = true,
        int failureCount = 0,
        int? tenantId = null)
    {
        SubscriptionId = subscriptionId;
        Action = action;
        Reason = reason;
        Amount = amount;
        SendEmailNotification = sendEmailNotification;
        FailureCount = failureCount;
        Timestamp = DateTimeOffset.UtcNow;
        TenantId = tenantId;
    }
}

public enum PaymentAction
{
    Success,
    Failed,
    Retry,
    Refunded,
    PartialRefund
}