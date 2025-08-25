namespace ConnectFlow.Application.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, CancelSubscriptionResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IContextManager _contextManager;
    private readonly ISubscriptionManagementService _subscriptionManagementService;

    public CancelSubscriptionCommandHandler(IApplicationDbContext context, IPaymentService paymentService, IContextManager contextManager, ISubscriptionManagementService subscriptionManagementService)
    {
        _context = context;
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

        // Cancel subscription in Stripe
        var stripeSubscription = await _paymentService.CancelSubscriptionAsync(currentSubscription.PaymentProviderSubscriptionId, request.CancelImmediately, cancellationToken);

        // Update local subscription
        if (request.CancelImmediately)
        {
            currentSubscription.Status = SubscriptionStatus.Canceled;
            currentSubscription.CanceledAt = DateTimeOffset.UtcNow;
        }
        else
        {
            currentSubscription.CancelAtPeriodEnd = true;
            currentSubscription.CanceledAt = DateTimeOffset.UtcNow;
        }

        // Raise domain event
        currentSubscription.AddDomainEvent(new SubscriptionCancelledEvent(tenantId.Value, currentSubscription.Id, request.CancelImmediately));

        await _context.SaveChangesAsync(cancellationToken);

        var message = request.CancelImmediately ? "Subscription cancelled immediately. Access has been suspended." : "Subscription will be cancelled at the end of the current billing period.";

        var effectiveDate = request.CancelImmediately ? DateTimeOffset.UtcNow : currentSubscription.CurrentPeriodEnd;

        return new CancelSubscriptionResult
        {
            SubscriptionId = currentSubscription.Id,
            Status = "cancelled",
            Message = message,
            CancelledAt = currentSubscription.CanceledAt,
            EffectiveDate = effectiveDate
        };

    }
}