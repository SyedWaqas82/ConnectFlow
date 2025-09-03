using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.Queries.CalculateRefundCredit;

[AuthorizeTenant(false, true, Roles.TenantAdmin)]
public record CalculateRefundCreditQuery : IRequest<decimal>;

public class CalculateRefundCreditQueryHandler : IRequestHandler<CalculateRefundCreditQuery, decimal>
{
    private readonly ISubscriptionManagementService _subscriptionManagementService;
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IContextManager _contextManager;

    public CalculateRefundCreditQueryHandler(ISubscriptionManagementService subscriptionManagementService, IApplicationDbContext context, IPaymentService paymentService, IContextManager contextManager)
    {
        _subscriptionManagementService = subscriptionManagementService;
        _context = context;
        _paymentService = paymentService;
        _contextManager = contextManager;
    }

    public async Task<decimal> Handle(CalculateRefundCreditQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        Guard.Against.Null(tenantId, nameof(tenantId), "Tenant ID not found", () => new TenantNotFoundException("Tenant ID not found"));

        // Get current active subscription
        var currentSubscription = await _subscriptionManagementService.GetActiveSubscriptionAsync(tenantId.Value, cancellationToken);
        Guard.Against.Null(currentSubscription, nameof(currentSubscription), "Current subscription not found", () => new SubscriptionNotFoundException("Current subscription not found"));


        if (currentSubscription.Plan.Type == PlanType.Free)
        {
            return 0; // No credit if the current plan is free
        }

        if (string.IsNullOrEmpty(currentSubscription.PaymentProviderSubscriptionId))
        {
            throw new InvalidOperationException("The subscription does not have a valid payment subscription ID.");
        }

        var newPlan = await _context.Plans.FirstOrDefaultAsync(p => p.Id != currentSubscription.PlanId && p.IsActive && p.Type != PlanType.Free, cancellationToken);
        Guard.Against.Null(newPlan, nameof(newPlan), $"no inactive plans found", () => new PlanNotFoundException($"no inactive plans found"));

        // Calculate the expected credit using the payment service
        var creditAmount = await _paymentService.CalculateExpectedCreditAsync(currentSubscription.PaymentProviderSubscriptionId, newPlan.PaymentProviderPriceId);

        return creditAmount;
    }
}