namespace ConnectFlow.Domain.Entities;

public class Invoice : BaseAuditableEntity
{
    public string PaymentProviderInvoiceId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }
    public string Currency { get; set; } = "usd";
    public DateTimeOffset? PaidAt { get; set; }
    public string InvoicePdf { get; set; } = string.Empty;
    public string HostedInvoiceUrl { get; set; } = string.Empty;

    //Subscription
    public int SubscriptionId { get; set; }
    public Subscription Subscription { get; set; } = null!;
}