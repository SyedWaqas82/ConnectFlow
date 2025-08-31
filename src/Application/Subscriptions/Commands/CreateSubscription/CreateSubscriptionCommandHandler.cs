using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, Result<CreateSubscriptionResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IContextManager _contextManager;
    private readonly ISubscriptionManagementService _subscriptionManagementService;

    public CreateSubscriptionCommandHandler(IApplicationDbContext context, IPaymentService paymentService, IContextManager contextManager, ISubscriptionManagementService subscriptionManagementService)
    {
        _context = context;
        _paymentService = paymentService;
        _contextManager = contextManager;
        _subscriptionManagementService = subscriptionManagementService;
    }

    public async Task<Result<CreateSubscriptionResult>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        var applicationUserId = _contextManager.GetCurrentApplicationUserId();

        Guard.Against.Null(tenantId, nameof(tenantId), "Tenant ID not found", () => new ForbiddenAccessException("Tenant ID not found"));
        Guard.Against.Null(applicationUserId, nameof(applicationUserId), "Application User ID not found", () => new UnauthorizedAccessException("Application User ID not found"));

        // Get the tenant with Stripe customer ID
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId.Value, cancellationToken);
        Guard.Against.Null(tenant, nameof(tenant), "Tenant not found", () => new ForbiddenAccessException("Tenant not found"));
        Guard.Against.NullOrEmpty(tenant.PaymentProviderCustomerId, nameof(tenant.PaymentProviderCustomerId), "Tenant does not have a Stripe customer ID", () => new InvalidOperationException("Tenant does not have a Stripe customer ID"));

        // Verify the tenant is not already subscribed to a paid plan, other wise throw an exception if its not null
        var currentActiveSubscription = await _subscriptionManagementService.GetActiveSubscriptionAsync(tenantId.Value, cancellationToken);
        if (currentActiveSubscription != null && currentActiveSubscription.Plan.Type != PlanType.Free)
            throw new InvalidOperationException("Tenant is already subscribed to a plan.");

        // Verify the plan exists and is active
        var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == request.PlanId && p.IsActive, cancellationToken);
        Guard.Against.Null(plan, nameof(plan), $"Plan {request.PlanId} not found or inactive");

        if (currentActiveSubscription != null && currentActiveSubscription.Plan.Id == plan.Id)
            return Result<CreateSubscriptionResult>.Failure(null, "You are already subscribed to this plan.");

        if (plan.Type == PlanType.Free)
        {
            var freeSubscription = new Subscription
            {
                PaymentProviderSubscriptionId = $"free_{Guid.NewGuid()}",
                Status = SubscriptionStatus.Active,
                CurrentPeriodStart = DateTimeOffset.UtcNow,
                CurrentPeriodEnd = DateTimeOffset.UtcNow.AddYears(100), // Free plan never expires
                CancelAtPeriodEnd = false,
                PlanId = plan.Id,
                TenantId = tenant.Id
            };

            _context.Subscriptions.Add(freeSubscription);

            // Add event for new free subscription creation
            freeSubscription.AddDomainEvent(new SubscriptionStatusEvent(tenantId.Value, applicationUserId.Value, freeSubscription, SubscriptionAction.Create, $"Subscribed to free plan {plan.Name}", sendEmailNotification: true));

            await _context.SaveChangesAsync(cancellationToken);

            return Result<CreateSubscriptionResult>.Success(new CreateSubscriptionResult(), "Free subscription created successfully.");
        }
        else
        {
            if (currentActiveSubscription != null && currentActiveSubscription.Plan.Type == PlanType.Free)
            {
                _context.Subscriptions.Remove(currentActiveSubscription);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Create Stripe checkout session
            var session = await _paymentService.CreateCheckoutSessionAsync(tenant.PaymentProviderCustomerId, plan.PaymentProviderPriceId, request.SuccessUrl, request.CancelUrl,
            new Dictionary<string, string>
            {
                { "tenant_id", tenantId.Value.ToString() },
                { "plan_id", plan.Id.ToString() }
            }, cancellationToken);

            return Result<CreateSubscriptionResult>.Success(new CreateSubscriptionResult()
            {
                SessionId = session.Id,
                CheckoutUrl = session.Url
            }, "Checkout session created successfully.");
        }
    }
}