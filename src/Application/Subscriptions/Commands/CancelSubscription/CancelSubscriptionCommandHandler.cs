namespace ConnectFlow.Application.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, CancelSubscriptionResult>
{
    private readonly IPaymentService _paymentService;
    private readonly IContextManager _contextManager;
    private readonly ISubscriptionManagementService _subscriptionManagementService;

    public CancelSubscriptionCommandHandler(IPaymentService paymentService, IContextManager contextManager, ISubscriptionManagementService subscriptionManagementService)
    {
        _paymentService = paymentService;
        _contextManager = contextManager;
        _subscriptionManagementService = subscriptionManagementService;
    }

    public async Task<CancelSubscriptionResult> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        Guard.Against.Null(tenantId, nameof(tenantId));

        // Get current active subscription
        var currentSubscription = await _subscriptionManagementService.GetActiveSubscriptionAsync(tenantId.Value, cancellationToken);
        Guard.Against.Null(currentSubscription, nameof(currentSubscription));

        // Cancel subscription in Stripe - this will trigger a webhook that updates local state
        var stripeSubscription = await _paymentService.CancelSubscriptionAsync(currentSubscription.PaymentProviderSubscriptionId, request.CancelImmediately, cancellationToken);

        // Return result based on Stripe response - local state will be updated via webhook
        var message = request.CancelImmediately
            ? "Subscription cancellation request sent to Stripe. Local state will be updated via webhook."
            : "Subscription will be cancelled at the end of the current billing period. Local state will be updated via webhook.";

        var effectiveDate = request.CancelImmediately ? DateTimeOffset.UtcNow : currentSubscription.CurrentPeriodEnd;

        return new CancelSubscriptionResult
        {
            SubscriptionId = currentSubscription.Id,
            Status = "cancellation_requested",
            Message = message,
            CancelledAt = request.CancelImmediately ? DateTimeOffset.UtcNow : (DateTimeOffset?)null,
            EffectiveDate = effectiveDate
        };

    }
}