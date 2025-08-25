namespace ConnectFlow.Application.Subscriptions.Commands.ReactivateSubscription;

public class ReactivateSubscriptionCommandHandler : IRequestHandler<ReactivateSubscriptionCommand, ReactivateSubscriptionResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IContextManager _contextManager;
    private readonly ISubscriptionManagementService _subscriptionManagementService;

    public ReactivateSubscriptionCommandHandler(IApplicationDbContext context, IPaymentService paymentService, IContextManager contextManager, ISubscriptionManagementService subscriptionManagementService)
    {
        _context = context;
        _paymentService = paymentService;
        _contextManager = contextManager;
        _subscriptionManagementService = subscriptionManagementService;
    }

    public async Task<ReactivateSubscriptionResult> Handle(ReactivateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        Guard.Against.Null(tenantId, nameof(tenantId));

        // Get the subscription that is scheduled for cancellation
        var subscription = await _context.Subscriptions.Include(s => s.Plan).FirstOrDefaultAsync(s => s.TenantId == tenantId.Value && s.CancelAtPeriodEnd == true && s.Status == SubscriptionStatus.Active, cancellationToken);
        Guard.Against.Null(subscription, nameof(subscription), "No subscription found that can be reactivated");

        // Reactivate subscription in Stripe
        var stripeSubscription = await _paymentService.ReactivateSubscriptionAsync(subscription.PaymentProviderSubscriptionId, cancellationToken);

        // Update local subscription
        subscription.CancelAtPeriodEnd = false;
        subscription.CanceledAt = null;

        // Raise domain event
        subscription.AddDomainEvent(new SubscriptionReactivatedEvent(tenantId.Value, subscription.Id, subscription.Plan.Name));

        await _context.SaveChangesAsync(cancellationToken);

        return new ReactivateSubscriptionResult
        {
            SubscriptionId = subscription.Id,
            Status = "active",
            Message = "Subscription has been reactivated successfully"
        };
    }
}