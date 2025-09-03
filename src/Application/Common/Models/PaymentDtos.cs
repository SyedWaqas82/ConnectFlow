namespace ConnectFlow.Application.Common.Models;

public class PaymentCustomerDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
    public Dictionary<string, object> Data { get; set; } = new();
}

public class PaymentSubscriptionDto
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string PriceId { get; set; } = string.Empty; // Stripe price ID
    public DateTimeOffset CurrentPeriodStart { get; set; }
    public DateTimeOffset CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public DateTimeOffset? CanceledAt { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public Dictionary<string, object> Data { get; set; } = new();
}

public class PaymentCheckoutSessionDto
{
    public string Id { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
    public Dictionary<string, object> Data { get; set; } = new();
}

public class PaymentBillingPortalSessionDto
{
    public string Id { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
}

public class PaymentInvoiceDto
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string SubscriptionId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public Dictionary<string, object> Data { get; set; } = new();
}

public class PaymentEventDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int TenantId { get; set; }
    public int ApplicationUserId { get; set; }
    public Guid? ApplicationUserPublicId { get; set; }
    public string ObjectId { get; set; } = string.Empty;
    public string SubscriptionId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public DateTimeOffset Created { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}

public class PaymentUsageRecordDto
{
    public string Id { get; set; } = string.Empty;
    public long Quantity { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}

public enum PaymentInvoiceStatus
{
    Succeeded,
    Failed,
    Created,
    Finalized,
    Paid
}