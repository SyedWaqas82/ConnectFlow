using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Subscriptions.Commands.UpdateSubscription;

public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, Result<UpdateSubscriptionResult>>
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

    public async Task<Result<UpdateSubscriptionResult>> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        Guard.Against.Null(tenantId, nameof(tenantId), "Tenant ID not found", () => new TenantNotFoundException("Tenant ID not found"));

        // Get current active subscription
        var currentActiveSubscription = await _subscriptionManagementService.GetActiveSubscriptionAsync(tenantId.Value, cancellationToken);
        Guard.Against.Null(currentActiveSubscription, nameof(currentActiveSubscription), "Current subscription not found", () => new SubscriptionNotFoundException("Current subscription not found"));

        var currentPlan = currentActiveSubscription.Plan;

        // Get the new plan
        var newPlan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == request.NewPlanId && p.IsActive, cancellationToken);
        Guard.Against.Null(newPlan, nameof(newPlan), $"Plan {request.NewPlanId} not found or inactive", () => new PlanNotFoundException($"Plan {request.NewPlanId} not found or inactive"));

        if (newPlan.Id == currentPlan.Id)
            return Result<UpdateSubscriptionResult>.Failure(null, "You are already subscribed to this plan.");

        if (newPlan.Type == PlanType.Free)
            return Result<UpdateSubscriptionResult>.Failure(null, "Cannot switch to a free plan using this method. Please cancel the current subscription and create a new free subscription.");

        if (currentPlan.Type == PlanType.Free)
        {
            return Result<UpdateSubscriptionResult>.Failure(null, "Cannot switch from a free plan using this method. Please create a new paid subscription.");
        }

        // Update subscription in Stripe - this will trigger a webhook that updates local state
        var stripeSubscription = await _paymentService.UpdateSubscriptionAsync(currentActiveSubscription.PaymentProviderSubscriptionId, newPlan.PaymentProviderPriceId, metadata: new Dictionary<string, string>
                {
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

        return Result<UpdateSubscriptionResult>.Success(new UpdateSubscriptionResult
        {
            SubscriptionId = currentActiveSubscription.Id,
            Status = "update_requested"
        }, message);
    }
}