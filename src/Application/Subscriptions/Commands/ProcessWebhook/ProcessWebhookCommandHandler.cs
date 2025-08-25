using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Subscriptions.Commands.ProcessWebhook;

public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, ProcessWebhookResult>
{
    private readonly ILogger<ProcessWebhookCommandHandler> _logger;
    private readonly IPaymentService _paymentService;
    private readonly IMediator _mediator;

    public ProcessWebhookCommandHandler(ILogger<ProcessWebhookCommandHandler> logger, IPaymentService paymentService, IMediator mediator)
    {
        _logger = logger;
        _paymentService = paymentService;
        _mediator = mediator;
    }

    public async Task<ProcessWebhookResult> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentEvent = await _paymentService.ProcessWebhookAsync(request.Body, request.Signature, cancellationToken);

            var processed = await ProcessEventAsync(paymentEvent, cancellationToken);

            return new ProcessWebhookResult
            {
                EventId = paymentEvent.Id,
                EventType = paymentEvent.Type,
                Processed = processed,
                Message = processed ? "Event processed successfully" : "Event not handled"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process webhook");
            return new ProcessWebhookResult
            {
                EventId = "unknown",
                EventType = "unknown",
                Processed = false,
                Message = ex.Message
            };
        }
    }

    private async Task<bool> ProcessEventAsync(PaymentEventDto paymentEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing Payment event {EventId} of type {EventType}", paymentEvent.Id, paymentEvent.Type);

        // For now, just log the event - the Infrastructure layer will handle the actual processing
        // This keeps the Application layer clean and focused on business logic
        return await Task.FromResult(true);
    }
}