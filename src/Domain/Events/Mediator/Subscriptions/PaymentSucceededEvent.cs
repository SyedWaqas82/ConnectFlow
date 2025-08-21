namespace ConnectFlow.Domain.Events.Mediator.Subscriptions;

public class PaymentSucceededEvent : BaseEvent
{
    public int SubscriptionId { get; }
    public string InvoiceId { get; }
    public decimal Amount { get; }
    public string Currency { get; }

    public PaymentSucceededEvent(int tenantId, int subscriptionId, string invoiceId, decimal amount, string currency)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        InvoiceId = invoiceId;
        Amount = amount;
        Currency = currency;
    }
}