using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Subscriptions.Commands.ProcessWebhook;

public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, ProcessWebhookResult>
{
    private readonly IPaymentService _paymentService;
    private readonly IPaymentWebhookEventHandlerService _paymentWebhookEventHandlerService;
    private readonly IContextManager _contextManager;
    private readonly ICacheService _cacheService;
    private readonly SubscriptionSettings _subscriptionSettings;

    public ProcessWebhookCommandHandler(IPaymentService paymentService, IPaymentWebhookEventHandlerService paymentWebhookEventHandlerService, IContextManager contextManager, ICacheService cacheService, IOptions<SubscriptionSettings> subscriptionSettings)
    {
        _paymentService = paymentService;
        _paymentWebhookEventHandlerService = paymentWebhookEventHandlerService;
        _contextManager = contextManager;
        _cacheService = cacheService;
        _subscriptionSettings = subscriptionSettings.Value;
    }

    public async Task<ProcessWebhookResult> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
    {
        var paymentEvent = await _paymentService.ProcessWebhookAsync(request.Body, request.Signature, cancellationToken);

        // Check for duplicate events using Redis cache
        var cacheKey = $"webhook_event_{paymentEvent.Id}";
        var existingEvent = await _cacheService.GetAsync<string>(cacheKey, cancellationToken);

        if (existingEvent != null)
        {
            return new ProcessWebhookResult
            {
                EventId = paymentEvent.Id,
                EventType = paymentEvent.Type,
                Processed = true,
                Message = "Event already processed (duplicate)"
            };
        }

        await _contextManager.InitializeContextAsync(paymentEvent.TenantId, paymentEvent.ApplicationUserId);

        var processed = await ProcessEventAsync(paymentEvent, cancellationToken);

        // Cache the event to prevent duplicate processing
        if (processed)
        {
            await _cacheService.SetAsync(cacheKey, "processed", TimeSpan.FromHours(_subscriptionSettings.EventIdempotencyCacheDuration), cancellationToken: cancellationToken);
        }

        return new ProcessWebhookResult
        {
            EventId = paymentEvent.Id,
            EventType = paymentEvent.Type,
            Processed = processed,
            Message = processed ? "Event processed successfully" : "Event not handled"
        };
    }

    private async Task<bool> ProcessEventAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken)
    {
        return paymentEvent.Type switch
        {
            "customer.subscription.created" => await _paymentWebhookEventHandlerService.HandleSubscriptionCreatedAsync(paymentEvent, cancellationToken),
            "customer.subscription.updated" => await _paymentWebhookEventHandlerService.HandleSubscriptionUpdatedAsync(paymentEvent, cancellationToken),
            "customer.subscription.deleted" => await _paymentWebhookEventHandlerService.HandleSubscriptionDeletedAsync(paymentEvent, cancellationToken),
            "invoice.payment_succeeded" => await _paymentWebhookEventHandlerService.HandlePaymentAsync(paymentEvent, PaymentInvoiceStatus.Succeeded, cancellationToken),
            "invoice.payment_failed" => await _paymentWebhookEventHandlerService.HandlePaymentAsync(paymentEvent, PaymentInvoiceStatus.Failed, cancellationToken),
            "invoice.created" => await _paymentWebhookEventHandlerService.HandlePaymentAsync(paymentEvent, PaymentInvoiceStatus.Created, cancellationToken),
            "invoice.finalized" => await _paymentWebhookEventHandlerService.HandlePaymentAsync(paymentEvent, PaymentInvoiceStatus.Finalized, cancellationToken),
            "invoice.paid" => await _paymentWebhookEventHandlerService.HandlePaymentAsync(paymentEvent, PaymentInvoiceStatus.Paid, cancellationToken),
            _ => await _paymentWebhookEventHandlerService.HandleUnknownEventAsync(paymentEvent, cancellationToken)
        };
    }
}