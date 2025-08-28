namespace ConnectFlow.Application.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, CreateSubscriptionResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IContextManager _contextManager;

    public CreateSubscriptionCommandHandler(IApplicationDbContext context, IPaymentService paymentService, IContextManager contextManager)
    {
        _context = context;
        _paymentService = paymentService;
        _contextManager = contextManager;
    }

    public async Task<CreateSubscriptionResult> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        Guard.Against.Null(tenantId, nameof(tenantId));

        // Get the tenant with Stripe customer ID
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId.Value, cancellationToken);
        Guard.Against.Null(tenant, nameof(tenant), "Tenant not found");
        Guard.Against.NullOrEmpty(tenant.PaymentProviderCustomerId, nameof(tenant.PaymentProviderCustomerId), "Tenant does not have a Stripe customer ID");

        // Verify the plan exists and is active
        var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == request.PlanId && p.IsActive, cancellationToken);
        Guard.Against.Null(plan, nameof(plan), $"Plan {request.PlanId} not found or inactive");

        // Create Stripe checkout session
        var session = await _paymentService.CreateCheckoutSessionAsync(tenant.PaymentProviderCustomerId, plan.PaymentProviderPriceId, request.SuccessUrl, request.CancelUrl,
            new Dictionary<string, string>
            {
                { "tenant_id", tenantId.Value.ToString() },
                { "plan_id", plan.Id.ToString() }
            }, cancellationToken);

        return new CreateSubscriptionResult
        {
            SessionId = session.Id,
            CheckoutUrl = session.Url
        };
    }
}