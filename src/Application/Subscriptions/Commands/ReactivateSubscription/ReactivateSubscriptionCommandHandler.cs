using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Subscriptions.Commands.ReactivateSubscription;

public class ReactivateSubscriptionCommandHandler : IRequestHandler<ReactivateSubscriptionCommand, Result<ReactivateSubscriptionResult>>
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

    public async Task<Result<ReactivateSubscriptionResult>> Handle(ReactivateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        Guard.Against.Null(tenantId, nameof(tenantId), "Tenant ID not found", () => new TenantNotFoundException("Tenant ID not found"));

        // Get current active subscription
        var currentSubscription = await _subscriptionManagementService.GetActiveSubscriptionAsync(tenantId.Value, cancellationToken);
        Guard.Against.Null(currentSubscription, nameof(currentSubscription), "No subscription found that can be reactivated", () => new SubscriptionNotFoundException("No subscription found that can be reactivated"));

        if (!currentSubscription.CancelAtPeriodEnd)
        {
            return Result<ReactivateSubscriptionResult>.Success(null, "The current subscription is already active.");
        }

        // Reactivate subscription in Stripe - this will trigger a webhook that updates local state
        var stripeSubscription = await _paymentService.ReactivateSubscriptionAsync(currentSubscription.PaymentProviderSubscriptionId, cancellationToken);

        // Return result based on Stripe response - local state will be updated via webhook
        return Result<ReactivateSubscriptionResult>.Success(new ReactivateSubscriptionResult
        {
            SubscriptionId = currentSubscription.Id,
            Status = "reactivation_requested",
        }, "Reactivation request sent successfully.");
    }
}