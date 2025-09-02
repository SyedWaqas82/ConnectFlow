using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IPaymentService
{
    // Customer Management
    Task<PaymentCustomerDto> CreateCustomerAsync(string email, string name, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
    Task<PaymentCustomerDto> UpdateCustomerAsync(string customerId, string? email = null, string? name = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
    Task<PaymentCustomerDto> GetCustomerAsync(string customerId, CancellationToken cancellationToken = default);

    // Subscription Management
    Task<PaymentSubscriptionDto> CreateSubscriptionAsync(string customerId, string priceId, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
    Task<PaymentSubscriptionDto> UpdateSubscriptionAsync(string subscriptionId, string? priceId = null, bool? cancelAtPeriodEnd = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
    Task<PaymentSubscriptionDto> CancelSubscriptionAsync(string subscriptionId, bool cancelImmediately = false, CancellationToken cancellationToken = default);
    Task<PaymentSubscriptionDto> ReactivateSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default);
    Task<PaymentSubscriptionDto> GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default);

    // Checkout Session Management
    Task<PaymentCheckoutSessionDto> CreateCheckoutSessionAsync(string customerId, string priceId, string successUrl, string cancelUrl, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
    Task<PaymentCheckoutSessionDto> GetCheckoutSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    // Billing Portal Management
    Task<PaymentBillingPortalSessionDto> CreateBillingPortalSessionAsync(string customerId, string returnUrl, CancellationToken cancellationToken = default);

    // Invoice Management
    Task<PaymentInvoiceDto> GetInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default);
    Task<List<PaymentInvoiceDto>> GetInvoicesAsync(string customerId, int limit = 10, CancellationToken cancellationToken = default);

    // Webhook Processing
    Task<PaymentEventDto> ProcessWebhookAsync(string body, string signature, CancellationToken cancellationToken = default);
}