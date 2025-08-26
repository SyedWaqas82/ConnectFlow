namespace ConnectFlow.Application.Subscriptions.Commands.UpdateSubscription;

public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, UpdateSubscriptionResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IContextManager _contextManager;
    private readonly ISubscriptionManagementService _subscriptionManagementService;

    public UpdateSubscriptionCommandHandler(IApplicationDbContext context, IPaymentService paymentService, IContextManager contextManager, ISubscriptionManagementService subscriptionManagementService)
    {
        _context = context;
        _paymentService = paymentService;
        _contextManager = contextManager;
        _subscriptionManagementService = subscriptionManagementService;
    }

    public async Task<UpdateSubscriptionResult> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        Guard.Against.Null(tenantId, nameof(tenantId));

        // Get current active subscription
        var currentSubscription = await _subscriptionManagementService.GetActiveSubscriptionAsync(tenantId.Value, cancellationToken);
        Guard.Against.Null(currentSubscription, nameof(currentSubscription));

        // Get the new plan
        var newPlan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == request.NewPlanId && p.IsActive, cancellationToken);
        Guard.Against.Null(newPlan, nameof(newPlan), $"Plan {request.NewPlanId} not found or inactive");

        var currentPlan = currentSubscription.Plan;

        // Update subscription in Stripe - this will trigger a webhook that updates local state
        var stripeSubscription = await _paymentService.UpdateSubscriptionAsync(currentSubscription.PaymentProviderSubscriptionId, newPlan.PaymentProviderPriceId, metadata: new Dictionary<string, string>
                {
                    { "tenant_id", tenantId.Value.ToString() },
                    { "plan_id", newPlan.Id.ToString() },
                    { "previous_plan_id", currentPlan.Id.ToString() }
                }, cancellationToken: cancellationToken);

        // Return result based on Stripe response - local state will be updated via webhook
        var isUpgrade = newPlan.Price > currentPlan.Price;
        var isDowngrade = newPlan.Price < currentPlan.Price;

        var message = isUpgrade
            ? "Subscription upgrade request sent to Stripe. Local state will be updated via webhook."
            : isDowngrade
                ? "Subscription downgrade request sent to Stripe. Local state will be updated via webhook."
                : "Subscription plan change request sent to Stripe. Local state will be updated via webhook.";

        return new UpdateSubscriptionResult
        {
            SubscriptionId = currentSubscription.Id,
            Status = "update_requested",
            Message = message
        };
    }
}