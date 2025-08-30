using ConnectFlow.Application.Common.Exceptions;

namespace ConnectFlow.Application.Subscriptions.Commands.ReactivateSubscription;

public class ReactivateSubscriptionCommandHandler : IRequestHandler<ReactivateSubscriptionCommand, ReactivateSubscriptionResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IContextManager _contextManager;

    public ReactivateSubscriptionCommandHandler(IApplicationDbContext context, IPaymentService paymentService, IContextManager contextManager)
    {
        _context = context;
        _paymentService = paymentService;
        _contextManager = contextManager;
    }

    public async Task<ReactivateSubscriptionResult> Handle(ReactivateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        Guard.Against.Null(tenantId, nameof(tenantId), "Tenant ID not found", () => new TenantNotFoundException("Tenant ID not found"));

        // Get the subscription that is scheduled for cancellation
        var subscription = await _context.Subscriptions.Include(s => s.Plan).FirstOrDefaultAsync(s => s.TenantId == tenantId.Value && s.CancelAtPeriodEnd == true && s.Status == SubscriptionStatus.Active, cancellationToken);
        Guard.Against.Null(subscription, nameof(subscription), "No subscription found that can be reactivated", () => new SubscriptionNotFoundException("No subscription found that can be reactivated"));

        // Reactivate subscription in Stripe - this will trigger a webhook that updates local state
        var stripeSubscription = await _paymentService.ReactivateSubscriptionAsync(subscription.PaymentProviderSubscriptionId, cancellationToken);

        // Return result based on Stripe response - local state will be updated via webhook
        return new ReactivateSubscriptionResult
        {
            SubscriptionId = subscription.Id,
            Status = "reactivation_requested",
            Message = "Subscription reactivation request sent to Stripe. Local state will be updated via webhook."
        };
    }
}