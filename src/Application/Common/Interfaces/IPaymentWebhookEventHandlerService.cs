using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Common.Interfaces;

/// <summary>
/// <summary>
/// Service for handling payment webhook event types.
/// </summary>
public interface IPaymentWebhookEventHandlerService
{
    /// <summary>
    /// Handles subscription created webhook events.
    /// </summary>
    Task<bool> HandleSubscriptionCreatedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Handles subscription updated webhook events.
    /// </summary>
    Task<bool> HandleSubscriptionUpdatedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Handles subscription deleted webhook events.
    /// </summary>
    Task<bool> HandleSubscriptionDeletedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Handles payment status webhook events.
    /// </summary>
    Task<bool> HandlePaymentAsync(PaymentEventDto paymentEvent, PaymentInvoiceStatus invoiceStatus, CancellationToken cancellationToken);

    /// <summary>
    /// Handles unknown or unhandled webhook events.
    /// </summary>
    Task<bool> HandleUnknownEventAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);
}