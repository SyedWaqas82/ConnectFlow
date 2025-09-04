namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

/// <summary>
/// Handles payment-related events and their consequences
/// </summary>
public class PaymentStatusEvent : BaseEvent
{
    public Subscription Subscription { get; }
    public PaymentAction Action { get; }
    public string Reason { get; }
    public decimal? Amount { get; }
    public bool SendEmailNotification { get; }
    public int FailureCount { get; }

    public PaymentStatusEvent(int tenantId, int applicationUserId, Subscription subscription, PaymentAction action, string reason, decimal? amount = null, bool sendEmailNotification = true, int failureCount = 0) : base(tenantId, applicationUserId)
    {
        Subscription = subscription;
        Action = action;
        Reason = reason;
        Amount = amount;
        SendEmailNotification = sendEmailNotification;
        FailureCount = failureCount;
    }
}

public enum PaymentAction
{
    Success,
    Failed
}