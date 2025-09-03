using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, Result<CancelSubscriptionResult>>
{
    private readonly IPaymentService _paymentService;
    private readonly IContextManager _contextManager;
    private readonly ISubscriptionManagementService _subscriptionManagementService;
    private readonly SubscriptionSettings _subscriptionSettings;

    public CancelSubscriptionCommandHandler(IPaymentService paymentService, IContextManager contextManager, ISubscriptionManagementService subscriptionManagementService, IOptions<SubscriptionSettings> subscriptionSettings)
    {
        _paymentService = paymentService;
        _contextManager = contextManager;
        _subscriptionManagementService = subscriptionManagementService;
        _subscriptionSettings = subscriptionSettings.Value;
    }

    public async Task<Result<CancelSubscriptionResult>> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        Guard.Against.Null(tenantId, nameof(tenantId), "Tenant ID not found", () => new TenantNotFoundException("Tenant ID not found"));

        // Get current active subscription
        var currentSubscription = await _subscriptionManagementService.GetActiveSubscriptionAsync(tenantId.Value, cancellationToken);
        Guard.Against.Null(currentSubscription, nameof(currentSubscription), "Current subscription not found", () => new SubscriptionNotFoundException("Current subscription not found"));

        //check if its a free subscription
        if (currentSubscription.Plan.Type == PlanType.Free)
        {
            // If it's a free subscription, just return a success result
            return Result<CancelSubscriptionResult>.Success(new CancelSubscriptionResult
            {
                SubscriptionId = currentSubscription.Id,
                Status = "cant cancel a free subscription",
                CancelledAt = null,
                EffectiveDate = null
            }, "Free subscriptions cannot be cancelled.");
        }

        if (!_subscriptionSettings.AllowImmediateCancellations && request.CancelImmediately)
        {
            return Result<CancelSubscriptionResult>.Failure(null, "Immediate cancellations are not allowed as per the cancellation policy.");
        }

        // Cancel subscription in Stripe - this will trigger a webhook that updates local state
        var stripeSubscription = await _paymentService.CancelSubscriptionAsync(currentSubscription.PaymentProviderSubscriptionId, request.CancelImmediately, cancellationToken);

        // Return result based on Stripe response - local state will be updated via webhook
        var message = request.CancelImmediately
            ? "Subscription cancellation request sent to Stripe. Local state will be updated via webhook."
            : "Subscription will be cancelled at the end of the current billing period. Local state will be updated via webhook.";

        var effectiveDate = request.CancelImmediately ? DateTimeOffset.UtcNow : currentSubscription.CurrentPeriodEnd;

        return Result<CancelSubscriptionResult>.Success(new CancelSubscriptionResult
        {
            SubscriptionId = currentSubscription.Id,
            Status = "cancellation_requested",
            CancelledAt = request.CancelImmediately ? effectiveDate : (DateTimeOffset?)null,
            EffectiveDate = effectiveDate
        }, message);
    }
}