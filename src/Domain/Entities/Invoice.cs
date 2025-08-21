namespace ConnectFlow.Domain.Entities;

public class Invoice : BaseAuditableEntity
{
    public string StripeInvoiceId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTimeOffset? PaidAt { get; set; }


    //Subscription
    public int SubscriptionId { get; set; }
    public Subscription Subscription { get; set; } = null!;
}