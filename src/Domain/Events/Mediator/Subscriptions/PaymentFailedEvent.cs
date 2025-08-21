namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class PaymentFailedEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public string InvoiceId { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public string FailureReason { get; }

    public PaymentFailedEvent(int tenantId, int subscriptionId, string invoiceId, decimal amount, string currency, string failureReason)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        InvoiceId = invoiceId;
        Amount = amount;
        Currency = currency;
        FailureReason = failureReason;
    }
}