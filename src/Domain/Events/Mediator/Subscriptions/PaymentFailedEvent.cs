namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class PaymentFailedEvent : BaseEvent
{
    public Subscription Subscription { get; }
    public PaymentFailureAttempt FailureAttempt { get; }
    public string? FailureReason { get; }
    public DateTimeOffset NextRetryDate { get; }
    public DateTimeOffset GracePeriodEndDate { get; }

    public PaymentFailedEvent(Subscription subscription, PaymentFailureAttempt failureAttempt, string? failureReason, DateTimeOffset nextRetryDate, DateTimeOffset gracePeriodEndDate)
    {
        Subscription = subscription;
        FailureAttempt = failureAttempt;
        FailureReason = failureReason;
        NextRetryDate = nextRetryDate;
        GracePeriodEndDate = gracePeriodEndDate;
    }
}