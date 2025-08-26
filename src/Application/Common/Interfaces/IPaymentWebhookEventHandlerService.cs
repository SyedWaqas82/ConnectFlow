using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Common.Interfaces;

/// <summary>
/// Service for handling individual webhook event types
/// </summary>
public interface IPaymentWebhookEventHandlerService
{
    /// <summary>
    /// Handles customer.subscription.created webhook events
    /// </summary>
    Task<bool> HandleSubscriptionCreatedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Handles customer.subscription.updated webhook events
    /// </summary>
    Task<bool> HandleSubscriptionUpdatedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Handles customer.subscription.deleted webhook events
    /// </summary>
    Task<bool> HandleSubscriptionDeletedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Handles invoice.payment_succeeded webhook events
    /// </summary>
    Task<bool> HandleInvoicePaymentSucceededAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Handles invoice.payment_failed webhook events
    /// </summary>
    Task<bool> HandleInvoicePaymentFailedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Handles invoice.finalized webhook events
    /// </summary>
    Task<bool> HandleInvoiceFinalizedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Handles checkout.session.completed webhook events
    /// </summary>
    Task<bool> HandleCheckoutSessionCompletedAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);

    /// <summary>
    /// Handles unknown/unhandled webhook events
    /// </summary>
    Task<bool> HandleUnknownEventAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken);
}